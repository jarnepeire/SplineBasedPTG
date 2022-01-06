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

        Handles.color = Color.white;
        Handles.DrawLine(line.p0, line.p1);
    }
}
