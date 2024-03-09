using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointClass : MonoBehaviour
{
    public int rowId;
    public int colId;
    public int GetIds()
    {
        return int.Parse(rowId.ToString() + colId.ToString());
    }
    
    public string GetIdsFormatted()
    {

        return rowId + "-" + colId; 
    }
}
