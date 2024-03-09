using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class DecorationCreatorScript : MonoBehaviour
{
    public GameObject prefabToPlace;
    public GameObject cornerPrefab;
    public float yOffset = 0.0f;

    public void CreateDecoration()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Vector3 topMiddle = CalculateTopMiddle(renderer);
            prefabToPlace.transform.localScale = new Vector3(prefabToPlace.transform.localScale.x,prefabToPlace.transform.localScale.y,transform.localScale.z);

            Instantiate(prefabToPlace, topMiddle, transform.rotation, transform.parent);
            GridCreatorScript gc = GetComponent<GridCreatorScript>();
            foreach (GameObject go in gc.gridPoints)
            {
                PointClass currentPoint = go.GetComponent<PointClass>();
                if (currentPoint.colId == gc.totoalColumns-1 && currentPoint.rowId == gc.totalRows-1)
                {
                    Instantiate(cornerPrefab, go.transform.position, transform.rotation,transform.parent.parent.Find("Corners"));
                }
            }
        }
    }

    Vector3 CalculateTopMiddle(Renderer renderer)
    {
        return new Vector3(renderer.bounds.center.x, renderer.bounds.max.y, renderer.bounds.center.z);
    }

}
