using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class Starmap : MonoBehaviour
{

    string fileName = "hygdata_v3.csv";

    int numToRead = 10000;

    public struct Star
    {
        public string name;
        public Vector3 position;
        public float magnitude;
    }

    Star[] stars;

    List<string> pickedLines = new List<string>();

    const float MAGNITUDE_LIMIT = 7;

    void Start()
    {
        float t = Time.realtimeSinceStartup;

        string[] lines = File.ReadAllLines(fileName);

        Debug.Log("Read lines in: " + (Time.realtimeSinceStartup - t));

        // Look for stars that are below magnitude
        for (int i = 2; i < lines.Length; i++) // start from 2 to skip caption line and Sun
        {
            var magStr = lines[i].Split(',')[13];

            float mag = float.Parse(magStr);
            if (i == 50) Debug.Log(mag);

            if (mag < MAGNITUDE_LIMIT)
                pickedLines.Add(lines[i]);
        }

        Debug.Log("Found " + pickedLines.Count + " stars");

        int starNum = pickedLines.Count;

        stars = new Star[starNum];

        for (int i = 0; i < starNum; i++)
        {
            var split = pickedLines[i].Split(',');

            Star star = new Star();
            star.magnitude = float.Parse(split[13]);
            float x = float.Parse(split[17]);
            float y = float.Parse(split[18]);
            float z = float.Parse(split[19]);

            star.position = new Vector3(x, z, y);

            stars[i] = star;
        }

        GenerateStarsAsGeometryMesh();
        //GenerateStarsAsPrefabs();
    }

    void GenerateStarsAsPrefabs()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            GameObject starGO = Instantiate(starPrefab);
            starGO.transform.position = stars[i].position.normalized * 100;
            starGO.transform.forward = stars[i].position.normalized;
            starGO.transform.localScale = Vector3.one * GetScaleFromMagnitude(stars[i].magnitude);
        }
    }

    void GenerateStarsAsGeometryMesh()
    {
        Mesh m = new Mesh();

        Vector3[] vertices = new Vector3[stars.Length];
        Color[] colors = new Color[stars.Length];

        int triLength = stars.Length + (3 - stars.Length % 3);
        int[] triangles = new int[stars.Length * 3];

        for (int i = 0; i < stars.Length; i++)
        {
            vertices[i] = stars[i].position.normalized * 100;
            colors[i] = new Color(0, 0, 0, GetScaleFromMagnitude(stars[i].magnitude));

            triangles[i * 3] = i;
            triangles[i * 3 + 1] = i;
            triangles[i * 3 + 2] = i;
        }

        m.vertices = vertices;
        m.colors = colors;
        m.triangles = triangles;

        gameObject.AddComponent<MeshFilter>().sharedMesh = m;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }

    public static float GetScaleFromMagnitude(float magnitude)
    {
        float size = 1 - (magnitude) * 0.1f;

        return Mathf.Clamp(size, 0.3f, 10);
    }

    public GameObject starPrefab;
    public Material material;
}
