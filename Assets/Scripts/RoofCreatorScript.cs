using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofCreatorScript : MonoBehaviour
{
    public List<GameObject> roofPoints;
    public Material roofMaterial;

    public void CreateRoofMesh()
    {
        GameObject buildingObj = transform.parent.parent.parent.gameObject;
        List<GameObject> wallsList =  buildingObj.GetComponent<WallCreatorScript>().wallsList;

        // Elég mindig csak egy sarkot hozzá adni mivel akkor szépen kijön minden pont ami nekünk kell.
        foreach (GameObject go in wallsList)
        {
            roofPoints.Add(go.transform.GetChild(0).GetComponent<WindowGridCreatorScript>().cornerPoints[2]);
        }
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[roofPoints.Count];
        for (int i = 0; i < roofPoints.Count; i++)
        {
            vertices[i] = roofPoints[i].transform.position;
        }
        List<int> triangles = Triangulate(vertices);
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        
        Vector2[] uvs = CalculateUVs(vertices);
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
        meshRenderer.material = roofMaterial;

        // Eltolni picit
        transform.localPosition = new Vector3(transform.position.x, -0.05f, transform.position.x);
    }

    List<int> Triangulate(Vector3[] vertices)
    {
        List<int> triangles = new List<int>();

        int n = vertices.Length;
        int[] V = new int[n];

        if (Area(vertices) > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return triangles;

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(vertices, u, v, w, nv, V))
            {
                int a, b, c, s, t;

                a = V[u];
                b = V[v];
                c = V[w];
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);

                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];

                nv--;

                count = 2 * nv;
                m = 0;
            }
            m++;
        }

        triangles.Reverse();
        return triangles;
    }

    float Area(Vector3[] vertices)
    {
        int n = vertices.Length;
        float A = 0.0f;

        for (int p = n - 1, q = 0; q < n; p = q++)
            A += vertices[p].x * vertices[q].z - vertices[q].x * vertices[p].z;

        return (A * 0.5f);
    }

    bool Snip(Vector3[] vertices, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = vertices[V[u]];
        Vector3 B = vertices[V[v]];
        Vector3 C = vertices[V[w]];

        if (Mathf.Epsilon > (((B.x - A.x) * (C.z - A.z)) - ((B.z - A.z) * (C.x - A.x))))
            return false;

        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;

            Vector3 P = vertices[V[p]];

            if (InsideTriangle(A, B, C, P))
                return false;
        }

        return true;
    }

    bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, az, bx, bz, cx, cz, apx, apz, bpx, bpz, cpx, cpz;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x;
        az = C.z - B.z;
        bx = A.x - C.x;
        bz = A.z - C.z;
        cx = B.x - A.x;
        cz = B.z - A.z;
        apx = P.x - A.x;
        apz = P.z - A.z;
        bpx = P.x - B.x;
        bpz = P.z - B.z;
        cpx = P.x - C.x;
        cpz = P.z - C.z;

        aCROSSbp = ax * bpz - az * bpx;
        cCROSSap = cx * apz - cz * apx;
        bCROSScp = bx * cpz - bz * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }

    Vector2[] CalculateUVs(Vector3[] vertices)
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minZ = Mathf.Infinity;
        float maxZ = Mathf.NegativeInfinity;

        for (int i = 0; i < vertices.Length; i++)
        {
            minX = Mathf.Min(minX, vertices[i].x);
            maxX = Mathf.Max(maxX, vertices[i].x);
            minZ = Mathf.Min(minZ, vertices[i].z);
            maxZ = Mathf.Max(maxZ, vertices[i].z);
        }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            float normalizedU = Mathf.InverseLerp(minX, maxX, vertices[i].x);
            float normalizedV = Mathf.InverseLerp(minZ, maxZ, vertices[i].z);

            uvs[i] = new Vector2(normalizedU, normalizedV);
        }

        return uvs;
    }
}
