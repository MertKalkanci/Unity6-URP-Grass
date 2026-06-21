using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public class GrassGenerator : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;

    [SerializeField] private int _chunkSize = 50000, countPerTriangle = 5;
    [SerializeField] private uint seed = 1234;
    [SerializeField] private float _grassOfset = 0.05f;

    private int _nativeArraylength;
    private int[] _triangles;
    private Vector3[] _positions;

    NativeArray<float3x3> _verticiesNative;
    NativeArray<float3> _positionsNative;

    private void Start()
    {
#if UNITY_SERVER
        Destroy(InstancedIndirectGrassRenderer.instance);
        return;
#endif
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Unity.Mathematics.Random _rng = new Unity.Mathematics.Random(seed);
        
        Mesh mesh = _meshFilter.sharedMesh;
        _triangles = mesh.triangles;
        _positions = mesh.vertices.Select(x => x).ToArray();
        for (int i = 0; i < _positions.Length; i++)
        {
            _positions[i].Scale(_meshFilter.transform.lossyScale);
            _positions[i] += _meshFilter.transform.position;
            if (i % _chunkSize == 0)
                yield return new WaitForEndOfFrame();
        }
        _nativeArraylength = _triangles.Length * countPerTriangle;
        // allocate a native array to store the positions
        _verticiesNative = new NativeArray<float3x3>(_nativeArraylength, Allocator.Persistent);
        _positionsNative = new NativeArray<float3>(_nativeArraylength, Allocator.Persistent);

        // load the positions into the native array
        for (int i = 0; i < _triangles.Length; i+=3)
        {
            for (int j = 0; j < countPerTriangle; j++)
            {
                _verticiesNative[i * countPerTriangle + j] = new float3x3(_positions[_triangles[i]], _positions[_triangles[i + 1]], _positions[_triangles[i + 2]]);
            }

            //if (i % _chunkSize == 0)
            //    yield return new WaitForEndOfFrame();
        }
        CalculatePositionsJob positionJob = new CalculatePositionsJob
        {
            grassOfset = _grassOfset,
            rng = _rng,
            Vertices = _verticiesNative,
            Positions = _positionsNative
        };
        
        JobHandle grassJobHandle = positionJob.Schedule(_nativeArraylength, 64);
        
        while (!grassJobHandle.IsCompleted)
        {
            yield return new WaitForEndOfFrame(); // Wait for the job to finish
        }
        //JobHandle.ScheduleBatchedJobs();
        grassJobHandle.Complete();

        /*
        do
        {
            yield return new WaitForEndOfFrame();
        } while (!grassJobHandle.IsCompleted);
        */
        Debug.Log("Grass generation Job done");
        
        // set the grass positions from the native array by converting it to a list
        List<Vector3> newPositionList = new List<Vector3>();
        for (int i = 0; i < _positionsNative.Length; i++)
        {
            newPositionList.Add(_positionsNative[i]);
            
            //if (i % _chunkSize == 0)
            //    yield return new WaitForEndOfFrame();
        }
        
        stopwatch.Stop();
        Debug.Log("passing grass positions to renderer, count:" + _nativeArraylength + " generated in " + stopwatch.ElapsedMilliseconds + " ms");
        //debug some positions
        InstancedIndirectGrassRenderer.instance.SetGrassPositions(newPositionList);
        //var list = _vertices.Select(x => x.position).ToList();

        //InstancedIndirectGrassRenderer.instance.SetGrassPositions(list);

        //Debug.Log(list.Count, gameObject);
        _verticiesNative.Dispose();
        _positionsNative.Dispose();
    }
}
public struct CalculatePositionsJob : IJobParallelFor
{
    [ReadOnly] public float grassOfset;
    [ReadOnly] public Unity.Mathematics.Random rng;
    [ReadOnly] public NativeArray<float3x3> Vertices;
    [WriteOnly] public NativeArray<float3> Positions;

    // calculate the positions between the vertices randomly
    public void Execute(int index)
    {
        var vertex = Vertices[index];
        var a = vertex.c2 - vertex.c0;
        var b = vertex.c1 - vertex.c0;
        var random = new float2(rng.NextFloat(0f,1f), rng.NextFloat(0f,1f));
        if (random.x + random.y > 1)
        {
            random.x = 1 - random.x;
            random.y = 1 - random.y;
        }
        var position = vertex.c0 + a * random.x + b * random.y;
        Positions[index] = position + grassOfset;
    }
}