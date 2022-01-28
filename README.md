# SplineBasedPTG
 Graduation work on spline-based procedural terrain generation, 2021-2022.
 
## How to navigate through the project
There is only 1 scene in the project, called the "PTGScene", and can be found under Assets -> Scenes -> PTGScene.  

Once the scene is open, in the hierarchy tab you can find the "Bezier Spline Editing" GameObject which enables the handles to edit the spline on which your procedural roads will be generated. 

![BezierObject](/Images/BezierSplineEditing.png)
![SplineEditor](/Images/SplineEditing.gif)

When running the game, you will see a procedurally generated road with trees on the sides being spawned. By editing the spline, you will see the road/trees change and transform along with the spline.

To edit the amount of trees spawned, you can click either of the "Procedural Placement" objects, and play around with the settings in the inspector window where you will find its script. 

![ProceduralPlacement](/Images/ProceduralPlacement.png)

The same applies to editing the road mesh, to make the polycount higher and subdivide the mesh more, you can click on the "Procedural Road Mesh" object and tweak its values in the inspector window where you will find its script.

![ProceduralRoadMesh](/Images/ProceduralRoadMesh.png)
