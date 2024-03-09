using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalconyCreatorScript : MonoBehaviour
{
    public GameObject balconyPrefab;
    public WindowCreatorScript windowCreatorScript;
    public GameObject container;
    public int chanceOfBalcony;
    public List<Window> windows = new List<Window>();
    
    private List<GameObject> windowPoints = new List<GameObject>();
    private List<GameObject> windowGoList = new List<GameObject>();
    private WindowGridCreatorScript windowGridCreatorScript;    
    private List<bool> windowPropPlaceable = new List<bool>();
    // To-Do random oszlopok

    public void CreateBalcony()
    {
        windowGridCreatorScript = GetComponent<WindowGridCreatorScript>();
        windowPoints = windowCreatorScript.windowPoints;
        windowGoList = windowCreatorScript.windowGoList;
        windowPropPlaceable = windowCreatorScript.windowPropPlaceable;
        
        List<int> windowsWithBalconies = UniqueRandomsWithChance(0, windowGridCreatorScript.columns-1, chanceOfBalcony);
        
        for (int j = 0; j < windowsWithBalconies.Count; j++)
        {
            for (int i = 0; i < windowGoList.Count; i++)
            {
                if (windowPoints[i].GetComponent<PointClass>().colId == windowsWithBalconies[j] && windowPoints[i].GetComponent<PointClass>().rowId > 0)
                {
                    Vector3 position = new Vector3(windowGoList[i].transform.position.x, windowGoList[i].transform.position.y - 0.6f ,windowGoList[i].transform.position.z);
                    GameObject balcony = Instantiate(balconyPrefab, position, transform.rotation,container.transform);
                    windows.Add(new Window(true,windowPoints[i].GetComponent<PointClass>()));
                    windowPropPlaceable[i] = false;
                }
            }
        }
    }
    
    static List<int> UniqueRandomsWithChance(int minValue, int maxValue, int chance)
    {
        int range = maxValue - minValue + 1;
        int count = (int)Mathf.Round((float)(range * chance / 100.0));

        if (count > range)
        {
            Debug.LogError("Chance is too high for the given range.");
        }

        List<int> result = new List<int>();
        HashSet<int> usedValues = new HashSet<int>();

        System.Random random = new System.Random();

        while (result.Count < count)
        {
            int randomValue = random.Next(minValue, maxValue + 1);

            if (!usedValues.Contains(randomValue))
            {
                usedValues.Add(randomValue);
                result.Add(randomValue);
            }
        }

        return result;
    }
    
    static List<int> UniqueRandoms(int minValue, int maxValue, int count)
    {
        if (count > maxValue - minValue + 1)
        {
            Debug.LogError("Count must be less than or equal to the range of unique integers.");
        }

        List<int> result = new List<int>();
        HashSet<int> usedValues = new HashSet<int>();

        System.Random random = new System.Random();

        while (result.Count < count)
        {
            int randomValue = random.Next(minValue, maxValue + 1);

            if (!usedValues.Contains(randomValue))
            {
                usedValues.Add(randomValue);
                result.Add(randomValue);
            }
        }

        return result;
    }
}
