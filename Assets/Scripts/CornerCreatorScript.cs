using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CornerCreatorScript : MonoBehaviour
{
    public GameObject cornerPrefab;
    public List<Vector3> cornerPoints;
    public List<GameObject> walls;


    public void FixCorners()
    {
        Transform cornerContainer = new GameObject("Corners").transform;
        cornerContainer.parent = walls[0].transform.parent;
        for (int i = 0; i < cornerPoints.Count; i++)
        {
            cornerPrefab.transform.localScale = new Vector3(cornerPrefab.transform.localScale.x,transform.localScale.y,cornerPrefab.transform.localScale.z);
            Instantiate(cornerPrefab, 
                new Vector3(cornerPoints[i].x, transform.position.y ,cornerPoints[i].z), 
                walls[i].transform.GetChild(0).rotation, 
                cornerContainer);
        }
    }
}
