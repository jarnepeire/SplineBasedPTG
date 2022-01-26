using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve : MonoBehaviour
{
    //Helper struct for a point along a bezier curve
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

    [Range(0.01f, 1f)]
    [SerializeField]
    public float tValue = 0f;

    [SerializeField]
    public Vector3[] ControlPoints = new Vector3[4];

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

    public Vector3 GetTangent(float t)
    {
        return (transform.TransformPoint(GetFirstDerivative(t)) - transform.position).normalized;
    }

    public Vector3 GetFirstDerivative(float t)
    {
        //Gathering each control point
        Vector3 p0 = GetControlPoint(0);
        Vector3 p1 = GetControlPoint(1);
        Vector3 p2 = GetControlPoint(2);
        Vector3 p3 = GetControlPoint(3);

        float t2 = t * t;
        float t3 = t2 * t;

        return
          p0 * (-3f * t3 + 6f * t - 3f)
        + p1 * (9f * t2 - 12f * t + 3f)
        + p2 * (-9f * t2 + 6f * t)
        + p3 * (t2);

    }

    public float GetApproximateLength(int samples = 8)
    {
        //Sample different points and calculate a distance
        Vector3[] points = new Vector3[samples];
        for (int i = 0; i < samples; ++i)
        {
            //-1 so that the final t value will be 1
            float t = i / (samples - 1);
            points[i] = GetBezierPoint(t).BezierPosition;
        }


        float dist = 0;
        //Here we count up the straight distances between the differents points sampled
        //And add them up 
        for (int i = 0; i < samples - 1; ++i)
        {
            Vector3 a = points[i];
            Vector3 b = points[i+1];
            dist += Vector3.Distance(a, b);
        }
        return dist;
    }
}