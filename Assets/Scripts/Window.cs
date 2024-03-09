using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window
{
    public bool isPlaced;
    public PointClass pointData;

    public Window(bool isPlaced, PointClass pointData)
    {
        this.isPlaced = isPlaced;
        this.pointData = pointData;
    }
}
