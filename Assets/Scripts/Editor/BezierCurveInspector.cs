using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;
    private const float directionScale = 0.75f;

    private void OnSceneGUI()
    {
        curve = target as BezierCurve;
        handleTransform = curve.transform;
        handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = Color.grey;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 point = handleTransform.TransformPoint(curve.GetControlPoint(i));
            Handles.SphereHandleCap(i, point, Quaternion.identity, 0.35f, EventType.Repaint);
        }

        //Draw tValue sphere on spline
        Handles.color = Color.red;
        BezierCurve.BezierPoint pointOnSpline = curve.GetBezierPoint_Optimized(curve.tValue);
        Vector3 posOnSpline = handleTransform.TransformPoint(pointOnSpline.BezierPosition);
        Handles.SphereHandleCap(5, posOnSpline, Quaternion.identity, 0.25f, EventType.Repaint);

        //Position handle
        //Handles.PositionHandle(testPoint.BezierPosition, testPoint.BezierRotation);


        Handles.color = Color.grey;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);

        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
    }
    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = handleTransform.TransformPoint(curve.GetPoint(0f));
        Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);
        for (int i = 1; i <= lineSteps; i++)
        {
            point = handleTransform.TransformPoint(curve.GetPoint(i / (float)lineSteps));
            Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps) * directionScale);
        }
    }
    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(curve.ControlPoints[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            curve.ControlPoints[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }


}
