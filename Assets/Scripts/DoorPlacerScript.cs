using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPlacerScript : MonoBehaviour
{
    public GameObject doorPrefab;
    private WindowCreatorScript windowCreatorScript;
    private WindowGridCreatorScript windowGridCreatorScript;
    private GameObject doorObject;
    private int randomPosition;
    
    public void PlaceDoor()
    {
        windowCreatorScript = GetComponent<WindowCreatorScript>();
        windowGridCreatorScript = GetComponent<WindowGridCreatorScript>();
        
        RemoveWindowsFromWall();
        PlaceDoorOntoRandomColumn();
        doorObject.transform.position = new Vector3(doorObject.transform.position.x,doorObject.transform.localScale.y/2,doorObject.transform.position.z);
        
        for (int i = 0; i < windowCreatorScript.windowPoints.Count; i++)
        {
            if (windowCreatorScript.windowPoints[i].GetComponent<PointClass>().rowId == 0 && windowCreatorScript.windowPoints[i].GetComponent<PointClass>().colId != randomPosition) 
            {
                windowCreatorScript.windowGoList[i].SetActive(true);
            }
        }
    }

    private void PlaceDoorOntoRandomColumn()
    {
        randomPosition = Random.Range(0, windowGridCreatorScript.columns);
        for (int i = 0; i < windowCreatorScript.windowPoints.Count; i++)
        {
            if (windowCreatorScript.windowPoints[i].GetComponent<PointClass>().colId == randomPosition &&
                windowCreatorScript.windowPoints[i].GetComponent<PointClass>().rowId == 0)
            {
                doorObject = Instantiate(doorPrefab, windowCreatorScript.windowPoints[i].transform.position, transform.rotation,
                    transform.parent);
                Debug.Log("Door placed onto: " + randomPosition + " column.");
            }
        }
    }

    private void RemoveWindowsFromWall()
    {
        for (int i = 0; i < windowCreatorScript.windowPoints.Count; i++)
        {
            if (windowCreatorScript.windowPoints[i].GetComponent<PointClass>().rowId == 0)
            {
                windowCreatorScript.windowGoList[i].SetActive(false);
            }
        }
    }
}
