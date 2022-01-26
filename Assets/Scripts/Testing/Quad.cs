using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{

    /*
         [0]       [2]
       0,1 |-------| 1,1
           |\      |
           | \     |
           |  \    |
           |   \   |
           |    \  |
           |     \ |
       0,0 |-------| 1,0
         [1]       [3] 
     */

    private void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        Mesh mesh = mf.sharedMesh;
        mesh.name = "Quad";

        //Creates the point specified to create a quad
        List<Vector3> vertexPoints = new List<Vector3>()
        {
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
            new Vector3(1, 0, -1),
            new Vector3(-1, 0, -1)
        };


        //Normal generation, point along Y-axis
        List<Vector3> normals = new List<Vector3>()
        {
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0)
        };

        List<Vector2> uvs = new List<Vector2>()
        {
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };

        //clockwise order
        int[] triangleIndices = new int[]
        {
            0,2,3,
            3,1,0
        };

        //Set mesh information
        mesh.Clear();
        mesh.SetVertices(vertexPoints);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.triangles = triangleIndices;
        mesh.SetTriangles(triangleIndices, 0);
    }
}
