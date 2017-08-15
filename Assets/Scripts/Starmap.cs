using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class Starmap : MonoBehaviour
{
    public float magnitudeLimit = 7;

    public Material material;

    const string DATABASE_FILENAME = "hygdata_v3.csv";

    public struct Star
    {
        public string name;
        public Vector3 position;
        public float magnitude;
        public float colorIndex;
    }

    Star[] stars;

    List<string> pickedLines = new List<string>();

    void Start()
    {
        float t = Time.realtimeSinceStartup;

        string[] lines = File.ReadAllLines(DATABASE_FILENAME);

        Debug.Log("Read lines in: " + (Time.realtimeSinceStartup - t));

        // Look for stars that are below magnitude
        for (int i = 2; i < lines.Length; i++) // start from 2 to skip caption line and Sun
        {
            var magStr = lines[i].Split(',')[13];

            float mag = float.Parse(magStr);
            if (i == 50) Debug.Log(mag);

            if (mag < magnitudeLimit)
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

            float ci = 2800;
            float.TryParse(split[16], out ci);
            star.colorIndex = ci;

            stars[i] = star;
        }

        GenerateStarsAsGeometryMesh();
    }

    void GenerateStarsAsPrefabs(GameObject prefab)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            GameObject starGO = Instantiate(prefab);
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

        int[] triangles = new int[stars.Length * 3];

        for (int i = 0; i < stars.Length; i++)
        {
            vertices[i] = stars[i].position.normalized * 100;

            colors[i] = GetColorFromColorIndex(stars[i].colorIndex);
            colors[i].a = GetScaleFromMagnitude(stars[i].magnitude);

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
        float size = 1 - (magnitude) * 0.2f;

        return Mathf.Clamp(size, 0.1f, 10);
    }

    public static Color GetColorFromColorIndex(float B_V)
    {
        return GetColorFromTemperature(GetTemperatureFromColorIndex(B_V));
    }

    public static float GetTemperatureFromColorIndex(float B_V)
    {
        // From https://en.wikipedia.org/wiki/Color_index#cite_note-PyAstronomy-6
        return 4600 * (1 / ((0.92f * B_V) + 1.7f) + 1 / ((0.92f * B_V) + 0.62f));
    }

    public static Color GetColorFromTemperature(float temp)
    {
        // from http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

        temp = temp / 100;

        // RED

        float r, g, b;

        if (temp <= 66)
        {
            r = 255;
        }
        else
        {
            r = temp - 60;
            r = 329.698727446f * (Mathf.Pow(r, -0.1332047592f));

            r = Mathf.Clamp(r, 0, 255);
        }

        // GREEN

        if (temp <= 66)
        {
            g = temp;
            g = 99.4708025861f * Mathf.Log(g) - 161.1195681661f;

            g = Mathf.Clamp(g, 0, 255);
        }
        else
        {
            g = temp - 60;
            g = 288.1221695283f * Mathf.Pow(g, -0.0755148492f);

            g = Mathf.Clamp(g, 0, 255);
        }

        // BLUE

        if (temp >= 66)
        {
            b = 255;
        }
        else
        {
            if (temp <= 19)
            {
                b = 0;
            }
            else
            {
                b = temp - 10;
                b = 138.5177312231f * Mathf.Log(b) - 305.0447927307f;

                b = Mathf.Clamp(b, 0, 255);
            }
        }

        return new Color(r / 255, g / 255, b / 255);
    }

}
