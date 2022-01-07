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

        public Vector3 BezierPosition;
        public Quaternion BezierRotation;
    }

    [Range(0, 1)]
    [SerializeField]
    public float tValue = 0f;

    [SerializeField]
    public Vector3[] ControlPoints = new Vector3[4];

    private Vector3 _startOffset;

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

   // public BezierCurve(Transform[] controlPoints, Vector3 startOffset)
   // {
   //     for (int i = 0; i < 4; ++i)
   //     {
   //         _controlPoints[i] = controlPoints[i];
   //     }
   //
   //     _startOffset = startOffset;
   // }
   // public void Awake()
   // {
   //     for (int i = 0; i < 4; ++i)
   //     {
   //         _controlPoints[i].position = new Vector3(_controlPoints[i].position.x + _startOffset.x, _controlPoints[i].position.y + _startOffset.y, _controlPoints[i].position.z + _startOffset.z);
   //         _controlPoints[i].parent = this.transform;
   //     }
   // }
    //public void OnDrawGizmos()
    //{
    //    for (int i = 0; i < 4; ++i)
    //    {
    //        Gizmos.DrawSphere(GetControlPoint(i), 0.05f);
    //    }

    //    Handles.DrawBezier(GetControlPoint(0), GetControlPoint(3), GetControlPoint(1), GetControlPoint(2), Color.white, EditorGUIUtility.whiteTexture, 1f);

    //    Gizmos.color = Color.red;
    //    //BezierPoint testPoint = GetBezierPoint(tValue);
    //    BezierPoint testPoint = GetBezierPoint_Optimized(tValue);
    //    Gizmos.DrawSphere(testPoint.BezierPosition, 0.025f);
    //    Gizmos.color = Color.white;


    //    Handles.PositionHandle(testPoint.BezierPosition, testPoint.BezierRotation);
    //}

    BezierPoint GetBezierPoint(float t)
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

    public BezierPoint GetBezierPoint_Optimized(float t)
    {
        //Gathering each control point
        Vector3 p0 = GetControlPoint(0);
        Vector3 p1 = GetControlPoint(1);
        Vector3 p2 = GetControlPoint(2);
        Vector3 p3 = GetControlPoint(3);

        //Using the quadratic formula instead of lerps
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 bezierPoint = p0 * (omt2 * omt) + 
                p1 * (3f * omt2 * t) +
                p2 * (3f * omt * t2) +
                p3 * (t2 * t);

        Vector3 tangent =
            p0 * (-omt2) +
            p1 * (3 * omt2 - 2 * omt) +
            p2 * (-3 * t2 + 2 * t) +
            p3 * (t2);
        tangent.Normalize();

        
        Vector3 binormal = Vector3.Cross(Vector3.up, tangent).normalized;
        Vector3 normal = Vector3.Cross(tangent, binormal);
        Quaternion bezierRotation = Quaternion.LookRotation(tangent, normal);

        return new BezierPoint(bezierPoint, bezierRotation);

    }

    public Vector3 GetPoint(float t)
    {
        //Gathering each control point
        Vector3 p0 = GetControlPoint(0);
        Vector3 p1 = GetControlPoint(1);
        Vector3 p2 = GetControlPoint(2);
        Vector3 p3 = GetControlPoint(3);

        //Using the quadratic formula instead of lerps
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 bezierPoint = p0 * (omt2 * omt) +
                p1 * (3f * omt2 * t) +
                p2 * (3f * omt * t2) +
                p3 * (t2 * t);
        return bezierPoint;
    }

    public Vector3 GetDirection(float t)
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

        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        return
            3f * omt2 * (p1 - p0) +
            6f * omt * t * (p2 - p1) +
            3f * t2 * (p3 - p2);
    }
}
