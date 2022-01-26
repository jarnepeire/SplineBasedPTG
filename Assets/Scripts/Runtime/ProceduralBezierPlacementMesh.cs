using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBezierPlacementMesh : MonoBehaviour
{

    [SerializeField]
    private BezierCurve _curve;

    [Range(2, 64)]
    [SerializeField]
    int Amount = 4;

    [SerializeField]
    private GameObject _placementMesh;

    [SerializeField]
    private Vector3 LocalOffset;

    [SerializeField]
    private bool _skipEdgingMeshes;

    private List<GameObject> PlacementMeshes = new List<GameObject>();
    // Update is called once per frame
    void Update()
    {
        GenerateProceduralPlacementMeshes();
    }

    void GenerateProceduralPlacementMeshes()
    {
        if (!_placementMesh)
            return;

        //Delete meshes if decreased amount
        if (PlacementMeshes.Count > Amount)
        {
            int meshesToDelete = PlacementMeshes.Count - Amount;
            for (int i = PlacementMeshes.Count - 1; i > PlacementMeshes.Count - meshesToDelete - 1; --i)
            {
                Destroy(PlacementMeshes[i]);
            }
            PlacementMeshes.RemoveRange(PlacementMeshes.Count - meshesToDelete, meshesToDelete);
        }

        //Spawn meshes if increased amount
        if (PlacementMeshes.Count < Amount)
        {
            int meshesNeeded = Amount - PlacementMeshes.Count;
            for (int i = 0; i < meshesNeeded; ++i)
            { 
                GameObject go = Instantiate(_placementMesh);
                PlacementMeshes.Add(go);
            }
        }

        //Setting all meshes visible 
        for (int i = 0; i < Amount; ++i)
        {
            PlacementMeshes[i].GetComponent<MeshRenderer>().enabled = true;
            PlacementMeshes[i].gameObject.SetActive(true);
        }

        //Orient meshes correctly along spline
        for (int meshIdx = 0; meshIdx < Amount; ++meshIdx)
        {  

            GameObject go = PlacementMeshes[meshIdx];

            float t = meshIdx / (Amount - 1f);
            BezierCurve.BezierPoint bp = _curve.GetBezierPoint(t);

            go.transform.position = bp.BezierPosition + _curve.transform.position + LocalOffset;
            go.transform.rotation = bp.BezierRotation;
            
        }

        if (_skipEdgingMeshes)
        {
           PlacementMeshes[0].GetComponent<MeshRenderer>().enabled = false;
           PlacementMeshes[0].gameObject.SetActive(false);

           PlacementMeshes[PlacementMeshes.Count-1].GetComponent<MeshRenderer>().enabled = false;
           PlacementMeshes[PlacementMeshes.Count-1].gameObject.SetActive(false);
        }

    }
}
