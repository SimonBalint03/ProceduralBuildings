using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class GridCreatorScript : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject cornerPointPrefab;
    public int totalRows = 0;
    public int totoalColumns = 0;
    public List<GameObject> gridPoints = new List<GameObject>();    // Ignores corner points hat vagy nem fogalmam sincs
    public List<GameObject> cornerPoints = new List<GameObject>();

    private GameObject cornerTopLeft;
    private GameObject cornerTopRight;
    private GameObject cornerBottomLeft;
    private GameObject cornerBottomRight;

    private BoxCollider col;
    private List<Vector3> boxColCorners = new List<Vector3>();

    public void GenerateGrid(int rows, int cols, Transform parent)
    {
        col = transform.GetComponent<BoxCollider>();

        var trans = col.transform;
        var min = col.center - col.size * 0.5f;
        var max = col.center + col.size * 0.5f;


        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, min.y, max.z)));
        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, max.y, max.z)));
        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, max.y, min.z)));
        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, min.y, min.z)));

        for (int i = 0; i < boxColCorners.Count; i++)
        {
            cornerPoints.Add(Instantiate(cornerPointPrefab, boxColCorners[i], Quaternion.identity, transform));
        }

        cornerBottomLeft = cornerPoints[0];
        cornerTopLeft = cornerPoints[1];
        cornerTopRight = cornerPoints[2];
        cornerBottomRight = cornerPoints[3];
        
        Vector3 rowStep = (cornerBottomRight.transform.position - cornerBottomLeft.transform.position) / (float)(rows - 1);
        Vector3 columnStep = (cornerTopLeft.transform.position - cornerBottomLeft.transform.position) / (float)(cols - 1);

        // Generate the grid - Csak valamit forditva csinalt szoval altalaban meg van cserelve az oszlop meg a sor:)
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = cornerBottomLeft.transform.position + (i * rowStep) + (j * columnStep);
                GameObject gridPoint = Instantiate(pointPrefab,position, Quaternion.identity, parent);
                gridPoint.transform.position = position;
                gridPoint.transform.localPosition = new Vector3(0, gridPoint.transform.localPosition.y, gridPoint.transform.localPosition.z);
                // Valami forditva van de nem akarok hozza nyulni
                gridPoint.GetComponent<PointClass>().rowId = j;
                gridPoint.GetComponent<PointClass>().colId = i;
                gridPoints.Add(gridPoint);
            }
        }
        
        totalRows = rows;
        totoalColumns = cols;
    }

    public void GenerateTotalGrid(int cols, int rows, Transform parent)
    {
        col = transform.GetComponent<BoxCollider>();

        var trans = col.transform;
        var min = col.center - col.size * 0.5f;
        var max = col.center + col.size * 0.5f;


        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, min.y, max.z)));
        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, max.y, max.z)));
        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, max.y, min.z)));
        boxColCorners.Add(trans.TransformPoint(new Vector3(min.x, min.y, min.z)));

        for (int i = 0; i < boxColCorners.Count; i++)
        {
            cornerPoints.Add(Instantiate(cornerPointPrefab, boxColCorners[i], Quaternion.identity, transform));
        }

        cornerBottomLeft = cornerPoints[0];
        cornerTopLeft = cornerPoints[1];
        cornerTopRight = cornerPoints[2];
        cornerBottomRight = cornerPoints[3];
        
        Vector3 rowStep = (cornerBottomRight.transform.position - cornerBottomLeft.transform.position) /
                          (float)(cols - 1);
        Vector3 columnStep = (cornerTopLeft.transform.position - cornerBottomLeft.transform.position) /
                             (float)(rows - 1);
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 position = cornerBottomLeft.transform.position + (i * rowStep) + (j * columnStep);
                GameObject gridPoint = Instantiate(pointPrefab, position, Quaternion.identity, parent);
                gridPoint.transform.position = position;
                gridPoint.transform.localPosition = new Vector3(0, gridPoint.transform.localPosition.y, gridPoint.transform.localPosition.z);
                gridPoint.GetComponent<PointClass>().rowId = j;
                gridPoint.GetComponent<PointClass>().colId = i;
                gridPoints.Add(gridPoint);
            }
        }
        
        totalRows = rows;
        totoalColumns = cols;
    }
}