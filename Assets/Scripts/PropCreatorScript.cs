using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropCreatorScript : MonoBehaviour
{
    public WindowCreatorScript windowCreatorScript;
    public GameObject container;
    private List<GameObject> windowPoints = new List<GameObject>();
    private List<GameObject> windowGoList = new List<GameObject>();
    private List<bool> windowPropPlaceable = new List<bool>();
    
    public void CreateProps(GameObject prop, int chance)
    {
        windowPoints = windowCreatorScript.windowPoints;
        windowGoList = windowCreatorScript.windowGoList;
        windowPropPlaceable = windowCreatorScript.windowPropPlaceable;

        for (int i = 0; i < windowPoints.Count; i++)
        {
            int random = Random.Range(0, 101);
            if (windowPropPlaceable[i] &&  random < chance && !TryGetComponent<DoorPlacerScript>(out DoorPlacerScript doorPlacerScript))
            {
                Vector3 position = new Vector3(windowGoList[i].transform.position.x, windowGoList[i].transform.position.y - 0.6f ,windowGoList[i].transform.position.z);
                Instantiate(prop, position, transform.rotation,container.transform);
                windowPropPlaceable[i] = false;
            }
            else if(windowPropPlaceable[i] && random < chance && windowPoints[i].GetComponent<PointClass>().rowId != 0)
            {
                Vector3 position = new Vector3(windowGoList[i].transform.position.x, windowGoList[i].transform.position.y - 0.6f ,windowGoList[i].transform.position.z);
                Instantiate(prop, position, transform.rotation,container.transform);
                windowPropPlaceable[i] = false;
            }
            // Jo kis debug ha kesobb kell:
            // Debug.Log("Point: " + windowPoints[i].GetComponent<PointClass>().GetIdsFormatted() + " Placeable: " + windowPropPlaceable[i] + " On: " + transform.parent.name);
        }
    }
}
