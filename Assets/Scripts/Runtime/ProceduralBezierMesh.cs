using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBezierMesh : MonoBehaviour
{
    [SerializeField]
    private BezierCurve _curve;

    [Range(2, 64)]
    [SerializeField]
    int IterationCount = 8;

    [SerializeField]
    public Mesh2D Shape2D;

    private Mesh _mesh;

    // Start is called before the first frame update
    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        _mesh = mf.sharedMesh;
        _mesh.name = "Procedural Bezier Mesh";
    }

    // Update is called once per frame
    void Update()
    {
        GenerateProceduralGeometry();
    }

    private void GenerateProceduralGeometry()
    {
        //Vertices
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        float uSpan = Shape2D.CalculateUSpan();

        for (int iteration = 0; iteration < IterationCount; ++iteration)
        {
            float t = iteration / (IterationCount - 1f);
            BezierCurve.BezierPoint bp = _curve.GetBezierPoint(t);

            for (int j = 0; j < Shape2D.vertices.Length; ++j)
            {
                verts.Add(bp.TransformPointToWorld(Shape2D.vertices[j].point) + _curve.transform.position);
                normals.Add(bp.TransformDirToWorld(Shape2D.vertices[j].normal));

                //using T as our V coordinate to stretch along the line
                uvs.Add(new Vector2(Shape2D.vertices[j].u, t * (_curve.GetApproximateLength() / uSpan)));
            }
        }

        //Triangles
        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < IterationCount - 1; ++i)
        {
            //For each 
            int rootIdx = i * Shape2D.vertices.Length;
            int rootIdxNext = (i + 1) * Shape2D.vertices.Length;

            for (int line = 0; line < Shape2D.lineIndices.Length; line += 2)
            {
                int lineIdx0 = Shape2D.lineIndices[line];
                int lineIdx1 = Shape2D.lineIndices[line + 1];

                int currIdx0 = rootIdx + lineIdx0;
                int currIdx1 = rootIdx + lineIdx1;

                int nextIdx0 = rootIdxNext + lineIdx0;
                int nextIdx1 = rootIdxNext + lineIdx1;

                //Connect each vertex at the idx per 2D shape 
                triangleIndices.Add(currIdx0);
                triangleIndices.Add(nextIdx0);
                triangleIndices.Add(nextIdx1);

                triangleIndices.Add(currIdx0);
                triangleIndices.Add(nextIdx1);
                triangleIndices.Add(currIdx1);
            }
        }

        _mesh.Clear();
        _mesh.SetVertices(verts);
        _mesh.SetNormals(normals);
        _mesh.SetUVs(0, uvs);
        _mesh.SetTriangles(triangleIndices, 0);
    }
}
