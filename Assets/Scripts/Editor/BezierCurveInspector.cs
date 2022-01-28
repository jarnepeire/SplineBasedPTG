using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;
    private const float directionScale = 0.75f;

    private bool sceneIsAdded = false;

    private void Awake()
    {
        sceneIsAdded = true;
        SceneView.duringSceneGui += CustomOnSceneGUI;
    }

    private void OnDestroy()
    {
        if (Application.isEditor)
        {
            if (curve == null)
            {
                SceneView.duringSceneGui -= CustomOnSceneGUI;
            }
         }
    }

    private void OnEnable()
    {
        if (!sceneIsAdded)
        {
            SceneView.duringSceneGui += CustomOnSceneGUI;
            sceneIsAdded = true;
        }
    }

    private void OnDisable()
    {
        if (sceneIsAdded)
        {
            SceneView.duringSceneGui -= CustomOnSceneGUI;
            sceneIsAdded = false;
        }
    }


    private void CustomOnSceneGUI(SceneView sceneview)
    {
        //A target variable is set to the object to be drawn when OnSceneGUI is called.
        //We can cast this variable to a BezierCurve and then use the Handles utility class to draw a line between our points.
        curve = target as BezierCurve;

        if (!curve)
            return;

        //We need to take moving, rotating, and scaling into account relative to our game object
        handleTransform = curve.transform;

        //Besides the line, we can also show actual position handles for the two points
        //We can use Unity's pivot rotation mode to determine the rotation accordingly
        handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

        //Visualize handles for control points
        Vector3 p0 = DrawHandleForControlPoint(0);
        Vector3 p1 = DrawHandleForControlPoint(1);
        Vector3 p2 = DrawHandleForControlPoint(2);
        Vector3 p3 = DrawHandleForControlPoint(3);

        //Visualize spheres for control points
        Handles.color = Color.grey;
        for (int i = 0; i < 4; ++i)
        {
            Vector3 point = handleTransform.TransformPoint(curve.GetControlPoint(i));
            Handles.SphereHandleCap(i, point, Quaternion.identity, 0.35f, EventType.Repaint);
        }

        //Draw lines between control points
        Handles.color = Color.grey;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);

        //Draw tValue sphere on spline
        Handles.color = Color.red;
        BezierCurve.BezierPoint pointOnSpline = curve.GetBezierPoint(curve.tValue);
        Vector3 posOnSpline = handleTransform.TransformPoint(pointOnSpline.BezierPosition);
        Handles.SphereHandleCap(5, posOnSpline, Quaternion.identity, 0.25f, EventType.Repaint);

        //Draws full bezier spline 
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

    }



    private void DrawPointAsSphere(Vector3 point, int idx)
    {
        Handles.SphereHandleCap(idx, handleTransform.TransformPoint(point), Quaternion.identity, 0.25f, EventType.Repaint);
    }

    /* Draws and returns requested point in the correct world space */
    private Vector3 DrawHandleForControlPoint(int index)
    {
        //To make the handles work, we need to assign their results back into the line
        //As handles are in world space, we need to convert them back to the line's local space
        Vector3 point = handleTransform.TransformPoint(curve.ControlPoints[index]);

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            //To support undo functionality
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            
            //Set control point back to transformed point
            curve.ControlPoints[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }

}
