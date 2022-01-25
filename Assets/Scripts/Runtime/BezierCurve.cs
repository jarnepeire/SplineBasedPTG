using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve : MonoBehaviour
{

    public struct BezierPoint
    {
        public BezierPoint(Vector3 pos, Quaternion rot)
        {
            BezierPosition = pos;
            BezierRotation = rot;
        }

        public Vector3 TransformPointToWorld(Vector3 localPoint)
        {
            return (BezierRotation * localPoint) + BezierPosition;
        }
        public Vector3 TransformDirToWorld(Vector3 localPoint)
        {
            return (BezierRotation * localPoint);
        }



        public Vector3 BezierPosition;
        public Quaternion BezierRotation;
    }

    [SerializeField]
    public Mesh2D shape2D;

    [Range(2, 16)]
    [SerializeField]
    int iterationCount = 8;

    [Range(0.01f, 1f)]
    [SerializeField]
    public float tValue = 0f;

    [SerializeField]
    public Vector3[] ControlPoints = new Vector3[4];

    public GameObject ProceduralMeshObject;
    Mesh mesh;


    private void Awake()
    {
        MeshFilter mf = ProceduralMeshObject.GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        mesh = mf.sharedMesh;
        mesh.name = "Procgeom";
    }

    private void Update()
    {
        GenerateProceduralGeometry();
    }

    private void GenerateProceduralGeometry()
    {
        //Vertices
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        float uSpan = shape2D.CalculateUSpan();

        for (int iteration = 0; iteration < iterationCount; ++iteration)
        {
            float t = iteration / (iterationCount - 1f);
            BezierPoint bp = GetBezierPoint(t);

            for (int j = 0; j < shape2D.vertices.Length; ++j)
            {
                verts.Add(bp.TransformPointToWorld(shape2D.vertices[j].point));
                normals.Add(bp.TransformDirToWorld(shape2D.vertices[j].normal));

                //using T as our V coordinate to stretch along the line
                uvs.Add(new Vector2(shape2D.vertices[j].u, t * (GetApproximateLength() / uSpan))); 
            }
        }

        //Triangles
        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < iterationCount - 1; ++i)
        {
            //For each 
            int rootIdx = i * shape2D.vertices.Length;
            int rootIdxNext = (i+1) * shape2D.vertices.Length;

            for (int line = 0; line < shape2D.lineIndices.Length; line += 2)
            {
                int lineIdx0 = shape2D.lineIndices[line];
                int lineIdx1 = shape2D.lineIndices[line+1];

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

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangleIndices, 0);
    }

    public void Reset()
    {
        ControlPoints = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
    }

    public Vector3 GetControlPoint(int idx)
    {
        return ControlPoints[idx];
    }

    public BezierPoint GetBezierPoint(float t)
    {
        //Gathering each control point
        Vector3 p0 = GetControlPoint(0);
        Vector3 p1 = GetControlPoint(1);
        Vector3 p2 = GetControlPoint(2);
        Vector3 p3 = GetControlPoint(3);

        //Constructing bezier based on lerps
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        //Getting point/rotation values
        Vector3 bezierPosition = Vector3.Lerp(d, e, t);
        Vector3 fwd = e - d;
        fwd.Normalize();
        Quaternion bezierRot = Quaternion.LookRotation(fwd, Vector3.up);

        return new BezierPoint(bezierPosition, bezierRot);
    }

    public Vector3 GetPoint(float t)
    {
        //Gathering each control point
        Vector3 p0 = GetControlPoint(0);
        Vector3 p1 = GetControlPoint(1);
        Vector3 p2 = GetControlPoint(2);
        Vector3 p3 = GetControlPoint(3);


        //Constructing bezier based on lerps
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        //Getting point/rotation values
        Vector3 bezierPosition = Vector3.Lerp(d, e, t);
        return bezierPosition;

        ////Using the quadratic formula instead of lerps
        //float omt = 1f - t;
        //float omt2 = omt * omt;
        //float t2 = t * t;

        //Vector3 bezierPoint = p0 * (omt2 * omt) +
        //        p1 * (3f * omt2 * t) +
        //        p2 * (3f * omt * t2) +
        //        p3 * (t2 * t);
        //return bezierPoint;
    }

    public Vector3 GetTangent(float t)
    {
        return GetVelocity(t).normalized;
    }

    public Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(GetFirstDerivative(t)) - transform.position;
    }

    public Vector3 GetFirstDerivative(float t)
    {
        //Gathering each control point
        Vector3 p0 = GetControlPoint(0);
        Vector3 p1 = GetControlPoint(1);
        Vector3 p2 = GetControlPoint(2);
        Vector3 p3 = GetControlPoint(3);

        //float omt = 1f - t;
        //float omt2 = omt * omt;
        //float t2 = t * t;

        //return
        //    3f * omt2 * (p1 - p0) +
        //    6f * omt * t * (p2 - p1) +
        //    3f * t2 * (p3 - p2);

        float t2 = t * t;
        float t3 = t2 * t;

        return
          p0 * (-3f * t3 + 6f * t - 3f)
        + p1 * (9f * t2 - 12f * t + 3f)
        + p2 * (-9f * t2 + 6f * t)
        + p3 * (t2);

    }

    float GetApproximateLength(int precision = 8)
    {
        //sample points and calculate a distance
        Vector3[] points = new Vector3[precision];
        for (int i = 0; i < precision; ++i)
        {
            //-1 so that the final t value will be 1
            float t = i / (precision - 1);

            points[i] = GetBezierPoint(t).BezierPosition;
        }

        float dist = 0;
        for (int i = 0; i < precision - 1; ++i)
        {
            Vector3 a = points[i];
            Vector3 b = points[i+1];
            dist += Vector3.Distance(a, b);
        }
        return dist;
    }
}

/*
 * 
 *         Vector3 bezierPoint =
        p0 * (-t3 + 3f * t2 - 3f * t) +
        p1 * (3f * t3 - 6f * t2 + 3f * t) +
        p2 * (-3f * t3 + 3f * t2) +
        p3 * (t3);
*/