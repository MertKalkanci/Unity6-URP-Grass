# Unity 6 URP Grass

A highly performant grass rendering and placement solution designed specifically for mobile games using Unity 6 Universal Render Pipeline (URP).

This project was originally derived from [ColinLeung-NiloCat/UnityURP-MobileDrawMeshInstancedIndirectExample](https://github.com/ColinLeung-NiloCat/UnityURP-MobileDrawMeshInstancedIndirectExample) and has been extensively updated for Unity 6 with new runtime placement feature using Burst optimizations for dynamic placement.

![Screenshot 1](GitHubScreenshots/Screenshot-1.png)

## Features

- **Unity 6 URP Compatible**: Fully updated and optimized for Unity 6.
- **Rendering Paths**: Works seamlessly with both **Forward+** and **Forward** rendering paths.
- **Shadow Support**: Fully supports casting and receiving shadows while maintaining high performance.
- **Complex Mesh Placement**: Automatically places grass at runtime on complex meshes (e.g., terrain provided as a mesh with a standard `MeshFilter`).
- **Burst Optimized**: Utilizes Unity's Burst compiler for extremely fast random grass placement during scene load, ensuring no noticeable lag when initializing large environments.
- **Mobile Ready**: Built from the ground up to be highly performant on mobile devices.

## Screenshots

![Screenshot 2](GitHubScreenshots/Screenshot-2.png)
![Screenshot 3](GitHubScreenshots/Screenshot-3.jpeg)
![Screenshot 4](GitHubScreenshots/Screenshot-4.png)

## In Action

This grass system is actively used in the mobile game **Jelly Apocalypse**. Check out the gameplay video here: 

[![Jelly Apocalypse](https://img.youtube.com/vi/6KOLPbRm-Ko/0.jpg)](https://www.youtube.com/watch?v=6KOLPbRm-Ko&list=PLMedcR5V-l8HGFa6n8EIdyYq9LUktiTXP&index=1)

*(Click the image to watch the video)*
