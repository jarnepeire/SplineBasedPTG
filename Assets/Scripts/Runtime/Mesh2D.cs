using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
    [System.Serializable]
    public class Vertex
    {
        public Vector2 point;
        public Vector2 normal;
        public float u;
    }

    public Vertex[] vertices;
    public int[] lineIndices;

    public float CalculateUSpan()
    {
        //the total length of U coordinates spanning across 
        float dist = 0;
        for (int i = 0; i < lineIndices.Length; i+=2)
        {
            Vector2 a = vertices[lineIndices[i]].point;
            Vector2 b = vertices[lineIndices[i+1]].point;
            dist += Vector2.Distance(a, b);
        }
        return dist;

    }
    
}
