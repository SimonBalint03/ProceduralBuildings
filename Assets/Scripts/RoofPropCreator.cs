using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoofPropCreator : MonoBehaviour
{
    public List<GameObject> roofProps; // List of roof props
    public int numberOfPrefabs = 3;  // Number to place
    public Transform container;
    
    private MeshFilter roofMeshFilter;
    private float roofMeshRadius;

    void Start()
    {
        roofMeshFilter = GetComponent<MeshFilter>();
        PlacePrefabsOnRoof();
    }

    void PlacePrefabsOnRoof()
    {
        if (roofProps.Count == 0)
        {
            Debug.LogWarning("Roof props not assigned!");
            return;
        }

        MeshCollider simplifiedCollider = gameObject.AddComponent<MeshCollider>();
        simplifiedCollider.sharedMesh = roofMeshFilter.mesh;
        int index = 0;

        while (index < numberOfPrefabs)
        {

            Vector3 randomLocalPoint = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
            Vector3 randomPointAboveRoof = Vector3.Scale(randomLocalPoint, roofMeshFilter.mesh.bounds.extents*2);

            randomPointAboveRoof = roofMeshFilter.transform.TransformPoint(randomPointAboveRoof);

            RaycastHit hit;
            Ray ray = new Ray(randomPointAboveRoof + Vector3.up * 100f, Vector3.down);
            //Debug.DrawRay(ray.origin, ray.direction * 200f, Color.red, 5f);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == roofMeshFilter.gameObject.layer)
                {
                    Vector3 intersectionPoint = hit.point;

                    float randomRotationY = Mathf.Round(Random.Range(0f, 3f)) * 90f;
                    Quaternion randomRotation = Quaternion.Euler(0f, randomRotationY, 0f);

                    GameObject randomRoofProp = SelectRandomFromList(roofProps);
                    intersectionPoint = new Vector3(intersectionPoint.x,intersectionPoint.y + 0.075f,intersectionPoint.z);
                    Instantiate(randomRoofProp, intersectionPoint, randomRotation,container);
                    index++;
                }
            }
        }
        Destroy(simplifiedCollider);
    }

    private GameObject SelectRandomFromList(List<GameObject> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }
}
