using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    private void OnSceneGUI()
    {
        //A target variable is set to the object to be drawn when OnSceneGUI is called.
        //We can cast this variable to a line and then use the Handles utility class to draw a line between our points.
        Line line = target as Line;

        //We need to take moving, rotating, and scaling into account relative to our game object
        Transform handleTransform = line.transform;

        //Besides the line, we can also show actual position handles for the two points
        //We can use Unity's pivot rotation mode to determine the rotation accordingly
        Quaternion handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);


        //To make the handles work, we need to assign their results back into the line
        //As handles are in world space, we need to convert them back to the line's local space
        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            //To support undo functionality
            Undo.RecordObject(line, "Move Points");
            EditorUtility.SetDirty(line);
            line.p0 = handleTransform.InverseTransformPoint(p0);
        }

        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Points");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
 
       



    }
}
