using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingCreatorScript : MonoBehaviour
{
    
    [Header("Debug Points")]
    public GameObject basePointPrefab;
    public GameObject gridPointPrefab;
    public GameObject cornerPointPrefab;
    public GameObject windowPointPrefab;
    
    
    [Header("Base Point Creator")]
    public List<Vector3> basePoints;
    private BasePointCreator basePointCreator;
    private Transform groundPointsObj;
    private List<GameObject> basePointGOs;
    
    
    [Header("Randomization Settings")]
    // boolean hogy random vagy a pontok alapjan tegye le
    public bool randomizeBuilding;
    [Range(3, 15)] public int randomBPoints;
    [Range(1, 200)] public int minimumFloors;
    [Range(2, 200)] public int maximumFloors;
    
    [Header("Location Settings Of Random Building")]
    // Terulet amiben a pontokat fogja generalni: area * area
    [Range(10, 100)] public int area;
    // Ez meg a kezdo x, y koordinate hozza hogy ne legyen egybe a ket epulet
    [Range(0, 100)] public int x_cord;
    [Range(0, 100)] public int y_cord;

    [Header("Wall Settings")]
    public GameObject wallUnitPrefab;
    public int numberOfFloors = 2;
    private WallCreatorScript wallCreatorScript;
    // Corner creator settings
    public GameObject cornerPrefab;
    private CornerCreatorScript cornerCreatorScript;
    private List<GameObject> walls = new List<GameObject>();

    [Header("Door Settings")] 
    public GameObject doorPrefab;
    private DoorPlacerScript doorPlacerScript;

    [Header("Grid Settings")]
    private GridCreatorScript gridCreatorScript;
    
    [Header("Window Settings")]
    public GameObject windowPrefab;
    [Range(1,10)] public int windowDensity;
    private WindowCreatorScript windowCreatorScript;
    private WindowGridCreatorScript windowGridCreatorScript;

    [Header("Roof Settings")]
    public Material roofMaterial;
    private RoofCreatorScript roofCreatorScript;
    [SerializeField] private List<GameObject> roofPoints = new List<GameObject>();

    [Header("Decor Settings")] 
    public GameObject decorPrefab;
    public GameObject decorCornerPrefab;
    private List<GameObject> targetPoints;
    private DecorationCreatorScript decorationCreatorScript;

    [Header("Prop Settings")] 
    public GameObject balconyPrefab;
    [Range(0, 100)] public int percentageOfBalcony;
    public List<GameObject> propsList;
    [Range(0, 100)] public int percentageOfWallProps;
    public List<GameObject> roofPropList;
    public int roofPropsToCreate;

    private PropCreatorScript propCreatorScript;
    private BalconyCreatorScript balconyCreatorScript;
    private RoofPropCreator roofPropCreator;
    
    
    [Header("Debug Settings")] 
    public bool showWindowPoints = true;
    public bool showTotalPoints = true;
    
    // Helpers
    [SerializeField] private List<GameObject> totalGridPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> windowGridPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> windowGameObjects = new List<GameObject>();
    private List<GameObject> propsContainers = new List<GameObject>();

    private void Update()
    {
        #region Update Debug

        if (!showWindowPoints)
        {
            foreach (GameObject go in windowGridPoints)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in windowGridPoints)
            {
                go.SetActive(true);
            }
        }
        
        if (!showTotalPoints)
        {
            foreach (GameObject go in totalGridPoints)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in totalGridPoints)
            {
                go.SetActive(true);
            }
        }
        #endregion
    }

    private void Start()
    {
        // Lerakjuk az alap pontokat
        CreateBasePointsAndApply();
        // Lerakjuk a falakat és elhelyezzük õket mateknak megfelelõen.
        CreateWalls();
        // Minden falra rárakjuk a gridCreatort és az megcsinálja a grideket.
        CreateGridOnWalls();
        // Megkeressük a griden a nekünk fontos pontokat és ablakokat rakunk rá.
        CreateWindows();
        // A corner pontok alapján szépen csinálunk egy mesht a tetõre.
        CreateRoofAndAssign();
        // Teszünk egy totalgridet ami a sarkok miatt kell.Az oszlopok miatt kell majd(még nincs ilyen).
        CreateTotalGrid();
        // A sarkokat elforgatjuk hogy az egyik fallal párhuazmosak legyenek.
        FixWallCorners();
        // Véletlenszerûen dekorációkat helyezünk az ablakok alá.
        CreateDecoration();
        // Elhelyezzük az ajtót az egyik falon.
        CreateAndPlaceDoor();
        // Teszünk erkélyekez megfelelõ helyekre.
        CreateBalconies();

        
        // To-Do: A lentibõl metódust csinálni
        // Create props
        for (int i = 0; i < wallCreatorScript.wallsList.Count; i++)
        {
            propCreatorScript = wallCreatorScript.wallsList[i].transform.GetChild(0).GetComponent<PropCreatorScript>();
            propCreatorScript.windowCreatorScript = wallCreatorScript.wallsList[i].transform.GetChild(0).GetComponent<WindowCreatorScript>();
            propCreatorScript.container = propsContainers[i];
            foreach (GameObject prop in propsList)
            {
                propCreatorScript.CreateProps(prop,percentageOfWallProps/propsList.Count);
            }
        }
        // Roof props
        
        roofPropCreator = groundPointsObj.Find("Roof").AddComponent<RoofPropCreator>();
        GameObject roofPropContainer = new GameObject("Props");
        roofPropContainer.transform.parent = roofPropContainer.transform;
        
        roofPropCreator.roofProps = roofPropList;
        roofPropCreator.numberOfPrefabs = roofPropsToCreate;
        roofPropCreator.container = roofPropContainer.transform;

        Debug.Log("Done");

    }

    private void CreateBalconies()
    {
        int index = 0;
        foreach (GameObject wall in wallCreatorScript.wallsList)
        {
            CreatePropsContainer(wall);

            balconyCreatorScript = wall.transform.GetChild(0).GetComponent<BalconyCreatorScript>();
            balconyCreatorScript.balconyPrefab = balconyPrefab;
            balconyCreatorScript.windowCreatorScript = wall.transform.GetChild(0).GetComponent<WindowCreatorScript>();
            balconyCreatorScript.container = propsContainers[index];
            balconyCreatorScript.chanceOfBalcony = percentageOfBalcony;

            balconyCreatorScript.CreateBalcony();
            index++;
        }
    }

    private void CreatePropsContainer(GameObject wall)
    {
        GameObject propsGo = new GameObject("Props");
        propsContainers.Add(Instantiate(propsGo, wall.transform));
        Destroy(propsGo);
    }

    private void CreateAndPlaceDoor()
    {
        doorPlacerScript = walls[3].transform.GetChild(0).AddComponent<DoorPlacerScript>();
        doorPlacerScript.doorPrefab = doorPrefab;
        
        doorPlacerScript.PlaceDoor();
    }

    private void CreateDecoration()
    {
        foreach (GameObject wall in wallCreatorScript.wallsList)
        {
            decorationCreatorScript = wall.transform.GetChild(0).GetComponent<DecorationCreatorScript>();
            decorationCreatorScript.prefabToPlace = decorPrefab;
            decorationCreatorScript.cornerPrefab = decorCornerPrefab;
            
            decorationCreatorScript.CreateDecoration();
        }
    }

    private void CreateTotalGrid()
    {
        foreach (GameObject wall in wallCreatorScript.wallsList)
        {
            gridCreatorScript = wall.transform.GetChild(0).GetComponent<GridCreatorScript>();
            gridCreatorScript.pointPrefab = gridPointPrefab;
            gridCreatorScript.cornerPointPrefab = cornerPointPrefab;
            gridCreatorScript.GenerateTotalGrid(
                Mathf.Max(((int)Mathf.Round(wall.transform.GetChild(0).localScale.z) + windowDensity-1), 3),
                Mathf.Max(((int)Mathf.Round(wall.transform.GetChild(0).localScale.y) + windowDensity-1), 2),
                wall.transform.GetChild(0));
            totalGridPoints.AddRange(gridCreatorScript.gridPoints);
        }
        
    }

    private void FixWallCorners()
    {
        for (int i = 0; i < groundPointsObj.childCount-1; i++)
        {
            cornerCreatorScript = groundPointsObj.GetChild(i).GetChild(0).GetComponent<CornerCreatorScript>();
            cornerCreatorScript.walls = wallCreatorScript.wallsList;
            cornerCreatorScript.cornerPrefab = cornerPrefab;
            foreach (Vector3 go in basePointCreator.basePoints)
            {
                cornerCreatorScript.cornerPoints.Add(go);
            }
        }
        cornerCreatorScript.FixCorners();
    }

    private void CreateBasePointsAndApply()
    {
        groundPointsObj = gameObject.transform.Find("Points").Find("GroundPoints");
        basePointCreator = groundPointsObj.AddComponent<BasePointCreator>();
        basePointCreator.basePoints = randomizeBuilding ? RandomBasePoints(randomBPoints) : basePoints;
        basePointCreator.basePointPrefab = basePointPrefab;

        basePointCreator.CreateBasePoint(groundPointsObj);
    }

    private void CreateWalls()
    {
        wallCreatorScript = transform.AddComponent<WallCreatorScript>();
        wallCreatorScript.numberOfFloors = randomizeBuilding ? UnityEngine.Random.Range(minimumFloors, maximumFloors) : numberOfFloors;
        wallCreatorScript.wallUnitPrefab = wallUnitPrefab;
        wallCreatorScript.CreateWalls();
        numberOfFloors = wallCreatorScript.numberOfFloors;

        walls = wallCreatorScript.wallsList;
    }
    private void CreateGridOnWalls()
    {
        for (int i = 0; i < groundPointsObj.childCount; i++)
        {
            Transform currentWall = groundPointsObj.GetChild(i).GetChild(0);
            windowGridCreatorScript = groundPointsObj.GetChild(i).GetChild(0).GetComponent<WindowGridCreatorScript>();

            windowGridCreatorScript.pointPrefab = windowPointPrefab;
            windowGridCreatorScript.cornerPointPrefab = cornerPointPrefab;
            windowGridCreatorScript.columns = Mathf.Max(((int)Mathf.Round(currentWall.localScale.z) + windowDensity - 4), 1);
            windowGridCreatorScript.rows = Mathf.Max(wallCreatorScript.numberOfFloors, 1);
            
            // Nem tudom miert kell forditva megadni neki..
            windowGridCreatorScript.GenerateGrid(windowGridCreatorScript.columns, windowGridCreatorScript.rows, currentWall.transform);
            // Hozza adjuk a totalpointshoz a generált pontokat.
            windowGridPoints.AddRange(windowGridCreatorScript.gridPoints);
        }
    }
    private void CreateWindows()
    {
        for (int i = 0; i < groundPointsObj.childCount; i++)
        {
            Transform currentWall = groundPointsObj.GetChild(i).GetChild(0);
            windowCreatorScript = groundPointsObj.GetChild(i).GetChild(0).GetComponent<WindowCreatorScript>();
            windowCreatorScript.windowPrefab = windowPrefab;
            windowCreatorScript.CreateWindows();
            
            windowGameObjects.AddRange(windowCreatorScript.windowGoList);
        }
        
    }
    private void CreateRoofAndAssign()
    {
        GameObject roofObj = new GameObject("Roof");
        roofObj.layer = 6;
        roofObj.transform.parent = groundPointsObj;
        roofCreatorScript = roofObj.AddComponent<RoofCreatorScript>();

        roofCreatorScript.roofPoints = roofPoints;
        roofCreatorScript.roofMaterial = roofMaterial;
        roofCreatorScript.CreateRoofMesh();

    }
    private List<Vector3> RandomBasePoints(int numberOfBPoints)
    {
        var basePointList = new List<Vector3>();
        for (int i = 0; i < numberOfBPoints; i++)
        {
            var attempts = 0;
            var maxAttemps = 50;
            Vector3 pointToAdd;
            do
            {
                pointToAdd = new Vector3(UnityEngine.Random.Range(x_cord, x_cord + area), 0, UnityEngine.Random.Range(y_cord, y_cord + area));
                attempts++;
                for (int j = 0; j < basePointList.Count; j++)
                { 
                    if (i + 1 < numberOfBPoints)
                    {
                        var tempList = new List<Vector3>
                        {
                            pointToAdd,
                            basePointList[basePointList.Count - 1]
                        };
                        if (IsPointOnLine(basePointList[j], tempList)) continue;
                    }
                    else
                    {
                        var tempList = new List<Vector3>
                        {
                            pointToAdd,
                            basePointList[0]
                        };
                        if (IsPointOnLine(basePointList[j], tempList)) continue;
                    }
                }
                if (attempts > maxAttemps)
                {
                    Debug.Log($"Point number {i + 1} failed to be created out of {numberOfBPoints}");
                    break;
                }
            }
            while (IsPointInPolygon(pointToAdd, basePointList)
            || HaveLineIntersection(pointToAdd, basePointList)
            || IsPointOnLine(pointToAdd, basePointList));
            if (attempts <= maxAttemps)
            {
                basePointList.Add(pointToAdd);
            }
        }
        return basePointList;
    }
    public static bool IsPointInPolygon(Vector3 point, List<Vector3> polygonVertices)
    {
        bool isInside = false;
        int j = polygonVertices.Count - 1;

        for (int i = 0; i < polygonVertices.Count; i++)
        {
            if ((polygonVertices[i].y < point.y && polygonVertices[j].y >= point.y || polygonVertices[j].y < point.y && polygonVertices[i].y >= point.y) &&
                (polygonVertices[i].x <= point.x || polygonVertices[j].x <= point.x))
            {
                if (polygonVertices[i].x + (point.y - polygonVertices[i].y) / (polygonVertices[j].y - polygonVertices[i].y) * (polygonVertices[j].x - polygonVertices[i].x) < point.x)
                {
                    isInside = !isInside;
                }
            }
            j = i;
        }
        return isInside;
    }
    public static bool DoLinesIntersect(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
    {
        line1Start = new Vector2(line1Start.x, line1Start.z);
        line1End = new Vector2(line1End.x, line1End.z);
        line2Start = new Vector2(line2Start.x, line2Start.z);
        line2End = new Vector2(line2End.x, line2End.z);
        Vector2 dir1 = line1End - line1Start;
        Vector2 dir2 = line2End - line2Start;


        float crossProduct1 = CrossProduct2D(line2Start - line1Start, dir1);
        float crossProduct2 = CrossProduct2D(line2End - line1Start, dir1);

        float crossProduct3 = CrossProduct2D(line1Start - line2Start, dir2);
        float crossProduct4 = CrossProduct2D(line1End - line2Start, dir2);

        return (crossProduct1 * crossProduct2 < 0) && (crossProduct3 * crossProduct4 < 0);
    }
    private static float CrossProduct2D(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
    public static bool HaveLineIntersection(Vector3 point, List<Vector3> pointList)
    {
        if (pointList.Count < 3) return false;

        for (int i = 0; i < pointList.Count - 1; i++)
        {
            if (DoLinesIntersect(pointList[i], pointList[i + 1], pointList[pointList.Count - 1], point))
            {
                return true;
            }

            if (i < pointList.Count - 2 && DoLinesIntersect(pointList[i], pointList[i + 1], pointList[0], point))
            {
                return true;
            }
        }
        if (DoLinesIntersect(pointList[pointList.Count - 2], pointList[pointList.Count - 1], pointList[0], point))
        {
            return true;
        }
        return false;
    }
    public static bool IsPointOnLine(Vector3 point, List<Vector3> pointList)
    {
        var point2d = new Vector2(point.x, point.z);
        if (pointList.Count < 2) return false;
        for (int i = 0; i < pointList.Count - 1; i++)
        {
            var a = new Vector2(pointList[i].x, pointList[i].z);
            var b = new Vector2(pointList[i + 1].x, pointList[i + 1].z);
            float distanceFromStart = Vector2.Distance(point2d, a);
            float distanceToEnd = Vector2.Distance(point2d, b);
            float lineLength = Vector2.Distance(a, b);
            if (Mathf.Approximately(distanceFromStart + distanceToEnd, lineLength))
            {
                return true;
            }
        }
        return false;
    }
}
