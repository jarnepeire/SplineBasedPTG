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
        SceneView.duringSceneGui -= CustomOnSceneGUI;
    }
    private void OnEnable()
    {
        if (!sceneIsAdded)
        {
            SceneView.duringSceneGui += CustomOnSceneGUI;
            sceneIsAdded = true;
        }
    }

    

    private void CustomOnSceneGUI(SceneView sceneview)
    {


        //A target variable is set to the object to be drawn when OnSceneGUI is called.
        //We can cast this variable to a BezierCurve and then use the Handles utility class to draw a line between our points.
        curve = target as BezierCurve;

        if (!curve || !curve.shape2D)
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

        //Draw out test debug values
        //float radius = 0.25f;
        //Handles.SphereHandleCap(6, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.right * 2f)), Quaternion.identity, radius, EventType.Repaint);
        //Handles.SphereHandleCap(7, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.right * 1f)), Quaternion.identity, radius, EventType.Repaint);
        //
        //Handles.SphereHandleCap(8, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.left * 2f)), Quaternion.identity, radius, EventType.Repaint);
        //Handles.SphereHandleCap(9, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.left * 1f)), Quaternion.identity, radius, EventType.Repaint);
        //
        //Handles.SphereHandleCap(10, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.up * 2f)), Quaternion.identity, radius, EventType.Repaint);
        //Handles.SphereHandleCap(11, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.up * 1f)), Quaternion.identity, radius, EventType.Repaint);

        ///ShowDirections();

        //Draws full bezier spline 
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);


        Vector3[] verts = curve.shape2D.vertices.Select(v => pointOnSpline.TransformPointToWorld(v.point)).ToArray();
        //Draw mesh outline
        for (int i = 0; i < curve.shape2D.lineIndices.Length; i += 2)
        {
            Vector3 a = handleTransform.TransformPoint(verts[curve.shape2D.lineIndices[i]]);
            Vector3 b = handleTransform.TransformPoint(verts[curve.shape2D.lineIndices[i + 1]]);
            Handles.DrawLine(a, b);
        }

        for (int i = 0; i < curve.shape2D.vertices.Length; ++i)
        {
            DrawPointAsSphere(pointOnSpline.TransformPointToWorld(curve.shape2D.vertices[i].point), 12 + i);
            //Handles.SphereHandleCap(11, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(Vector3.up * 1f)), Quaternion.identity, radius, EventType.Repaint);
           // Handles.SphereHandleCap(12 + i, handleTransform.TransformPoint(pointOnSpline.TransformToWorld(curve.shape2D.vertices[i].point)), Quaternion.identity, radius, EventType.Repaint);
        
        }
    }


    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = handleTransform.TransformPoint(curve.GetPoint(0f));
        Handles.DrawLine(point, point + curve.GetTangent(0f) * directionScale);
        for (int i = 1; i <= lineSteps; i++)
        {
            point = handleTransform.TransformPoint(curve.GetPoint(i / (float)lineSteps));
            Handles.DrawLine(point, point + curve.GetTangent(i / (float)lineSteps) * directionScale);
        }
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
