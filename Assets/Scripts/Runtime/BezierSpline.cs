using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
    public Transform[] allControlPoints = new Transform[4];
    private List<BezierCurve> allBezierCurves = new List<BezierCurve>();

    public void AddBezierCurve(BezierCurve bezierCurve)
    {
        // Because we want the spline to be continuous,
        // the last point of the previous curve is the same as the first point of the next curve
        // each extra curve adds three more points
        Transform controlPoint = allControlPoints[allControlPoints.Length - 1];
        Array.Resize(ref allControlPoints, allControlPoints.Length + 3);

        //Move new control points in X-direction and add to array of current points
        controlPoint.position = new Vector3(controlPoint.position.x + 1, controlPoint.position.y, controlPoint.position.z);
        allControlPoints[allControlPoints.Length - 3] = controlPoint;

        controlPoint.position = new Vector3(controlPoint.position.x + 1, controlPoint.position.y, controlPoint.position.z);
        allControlPoints[allControlPoints.Length - 2] = controlPoint;

        controlPoint.position = new Vector3(controlPoint.position.x + 1, controlPoint.position.y, controlPoint.position.z);
        allControlPoints[allControlPoints.Length - 1] = controlPoint;

    }
}
