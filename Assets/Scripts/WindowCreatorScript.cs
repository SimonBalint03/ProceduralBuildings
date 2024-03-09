using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WindowCreatorScript : MonoBehaviour
{
    public List<GameObject> windowPoints;
    public GameObject windowPrefab;
    public List<GameObject> windowGoList = new List<GameObject>();
    public List<bool> windowPropPlaceable = new List<bool>();

    private WindowGridCreatorScript windowGridCreatorScript;
    private void Awake()
    {
        windowGridCreatorScript = GetComponentInParent<WindowGridCreatorScript>();
    }

    public void CreateWindows()
    {
        windowPoints = FindNotEdgeGridPoints();
        InstantiateAndAlignWindows();
    }

    private void InstantiateAndAlignWindows()
    {
        GameObject temp = new GameObject("Windows");
        GameObject windowsGameObject = Instantiate(temp, transform.parent);
        Destroy(temp);
        for (int i = 0; i < windowPoints.Count; i++)
        {
            Quaternion rotation = transform.parent.GetChild(0).rotation;
            // 90 fokkal forgatni kell mert 
            float y = rotation.eulerAngles.y + 90f;
            windowGoList.Add(Instantiate(windowPrefab, windowPoints[i].transform.position, Quaternion.Euler(0f, y, 0f), windowsGameObject.transform));
            windowPropPlaceable.Add(true);
        }
    }

    private List<GameObject> FindNotEdgeGridPoints()
    {
        int count = windowGridCreatorScript.gridPoints.Count;
        int rows = windowGridCreatorScript.rows;
        int cols = windowGridCreatorScript.columns;

        List<GameObject> windows = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            PointClass point = windowGridCreatorScript.gridPoints[i].GetComponent<PointClass>();
            // Leszürjük a 0. sor és oszlopot és az utolsó sort és oszlopot.
            // Azert rows-1 mert 0-tól számolunk id-t. duh
            windows.Add(point.gameObject);
        }

        return windows;
    }
}
