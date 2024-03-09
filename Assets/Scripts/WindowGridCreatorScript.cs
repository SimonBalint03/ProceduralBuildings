using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class WindowGridCreatorScript : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject cornerPointPrefab;
    public int rows = 10;
    public int columns = 10;
    public List<GameObject> gridPoints = new List<GameObject>();
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


        if (cornerTopLeft == null || cornerTopRight == null || cornerBottomLeft == null || cornerBottomRight == null)
        {
            Debug.LogError("Please assign all four corner GameObjects.");
            return;
        }

        Vector3 rowStep = (cornerBottomRight.transform.position - cornerBottomLeft.transform.position) / (float)(rows + 1);
        Vector3 columnStep = (cornerTopLeft.transform.position - cornerBottomLeft.transform.position) / (float)(cols + 1);
        for (int i = 1; i < rows + 1; i++)
        {
            for (int j = 1; j < cols + 1; j++)
            {
                Vector3 position = cornerBottomLeft.transform.position + (i * rowStep) + (j * columnStep);
                GameObject gridPoint = Instantiate(pointPrefab,position, Quaternion.identity, parent);
                gridPoint.transform.position = position;
                gridPoint.transform.localPosition = new Vector3(0, gridPoint.transform.localPosition.y, gridPoint.transform.localPosition.z);
                gridPoint.GetComponent<PointClass>().rowId = j - 1;
                gridPoint.GetComponent<PointClass>().colId = i - 1;
                gridPoint.name = gridPoint.GetComponent<PointClass>().GetIdsFormatted();
                gridPoints.Add(gridPoint);
            }
        }
    }

}