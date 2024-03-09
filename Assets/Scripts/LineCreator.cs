using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public Material lineMaterial;
    public Material defaultMeshMaterial;

    public void DrawPolygon(List<GameObject> points, Color lineColor, bool doCreateMesh)
    {
        GameObject childObject;
        childObject = Instantiate(new GameObject("LineRenderer"), gameObject.transform);

        LineRenderer lineRenderer = childObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = points.Count;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.loop = true;


        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].transform.position);
        }

        if (doCreateMesh)
        {
            GenerateMeshAndAssign(points);
        }
    }

    public void DrawPolygon(List<GameObject> points, Color lineColor)
    {
        GameObject childObject = Instantiate(new GameObject("LineRenderer"), gameObject.transform);

        LineRenderer lineRenderer = childObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = points.Count;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.loop = true;


        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].transform.position);
        }
        
        GenerateMeshAndAssign(points);

    }

    public void GenerateMeshAndAssign(List<GameObject> points)
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i].transform.position;
        }

        int[] triangles = new int[(vertices.Length - 2) * 3];
        int index = 0;

        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = i;
            triangles[index++] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        mesh.uv = uvs;
        meshRenderer.material = defaultMeshMaterial;
        meshFilter.mesh = mesh;
    }

    public void GenerateMeshAndAssign(List<GameObject> points, GameObject componentParent)
    {
        MeshFilter meshFilter = componentParent.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = componentParent.gameObject.AddComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i].transform.position;
        }

        int[] triangles = new int[(vertices.Length - 2) * 3];
        int index = 0;

        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = i;
            triangles[index++] = i + 1;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }
        
        mesh.uv = uvs;
        meshRenderer.material = defaultMeshMaterial;
        meshFilter.mesh = mesh;

    }

    public void GenerateMeshAndAssign(List<GameObject> points, GameObject componentParent, Material material)
    {
        MeshFilter meshFilter = componentParent.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = componentParent.gameObject.AddComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i].transform.position;
        }

        int[] triangles = new int[(vertices.Length - 2) * 3];
        int index = 0;

        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = i;
            triangles[index++] = i + 1;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;

        for (int i = 0; i < vertices.Length; i++)
        {
            minX = Mathf.Min(minX, vertices[i].x);
            maxX = Mathf.Max(maxX, vertices[i].x);
            minY = Mathf.Min(minY, vertices[i].y);
            maxY = Mathf.Max(maxY, vertices[i].y);
        }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            float normalizedU = Mathf.InverseLerp(minX, maxX, vertices[i].x);
            float normalizedV = Mathf.InverseLerp(minY, maxY, vertices[i].y);

            uvs[i] = new Vector2(normalizedU, normalizedV);
        }
        
        mesh.uv = uvs;
        meshRenderer.material = material;
        meshFilter.mesh = mesh;

    }
}
