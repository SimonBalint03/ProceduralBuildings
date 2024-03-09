using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePointCreator : MonoBehaviour
{
    public List<Vector3> basePoints;
    public GameObject basePointPrefab;

    public void CreateBasePoint(Transform go)
    {
        for (int i = 0; i < basePoints.Count; i++)
        {
            basePointPrefab.name = "Wall_" + i;
            Instantiate(basePointPrefab, basePoints[i], Quaternion.identity, go);
        }
    }
}
