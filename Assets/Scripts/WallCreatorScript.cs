using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WallCreatorScript : MonoBehaviour
{
    public GameObject wallUnitPrefab;
    public int numberOfFloors;
    public List<GameObject> wallsList = new List<GameObject>();

    private List<GameObject> wallObjects = new List<GameObject>();
    private List<Vector3> distanceBetweenWalls = new List<Vector3>();
    private List<Transform> wallTransformList = new List<Transform>();
 
    public void CreateWalls()
    {
        numberOfFloors++;
        
        var wallsContainerList = gameObject.transform.Find("Points").Find("GroundPoints");
        var wallsCount = wallsContainerList.childCount;
        for (int i = 0; i < wallsCount; i++)
        {
            wallsList.Add(wallsContainerList.GetChild(i).gameObject);
        }
        foreach (GameObject wp in wallsList)
        {
            wallTransformList.Add(wp.transform);
        }

        for (int i = 0; i < wallsList.Count; i++)
        {
            wallObjects.Add(Instantiate(wallUnitPrefab, wallsList[i].transform));
        }
        for (int i = 0; i < wallObjects.Count - 1; i++)
        {
            wallObjects[i].transform.LookAt(wallObjects[i + 1].transform);
        }
        wallObjects[wallObjects.Count - 1].transform.LookAt(wallObjects[0].transform);
        for (int i = 0; i < wallObjects.Count - 1; i++)
        {
            distanceBetweenWalls.Add((wallObjects[i].transform.position - wallObjects[i + 1].transform.position));
        }
        distanceBetweenWalls.Add((wallObjects[wallObjects.Count - 1].transform.position - wallObjects[0].transform.position));

        for (int i = 0; i < wallObjects.Count - 1; i++)
        {
            wallObjects[i].transform.position -= distanceBetweenWalls[i] / 2;
        }
        wallObjects[wallObjects.Count - 1].transform.position -= distanceBetweenWalls[distanceBetweenWalls.Count - 1] / 2;

        for (int i = 0; i < wallObjects.Count; i++)
        {
            wallObjects[i].transform.localScale = new Vector3(
                wallObjects[i].transform.localScale.x,
                wallObjects[i].transform.localScale.y * numberOfFloors,
                Mathf.Sqrt(
                Mathf.Pow(Mathf.Abs(distanceBetweenWalls[i].x), 2) +
                Mathf.Pow(Mathf.Abs(distanceBetweenWalls[i].z), 2)
                ));
            wallObjects[i].transform.position = new Vector3(
                wallObjects[i].transform.position.x,
                wallObjects[i].transform.position.y +
                (wallObjects[i].transform.localScale.y / 2),
                wallObjects[i].transform.position.z
                );
        }

        numberOfFloors--;
    }
}