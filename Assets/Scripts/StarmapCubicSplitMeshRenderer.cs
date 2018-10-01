using UnityEngine;
using System.Collections.Generic;

public class StarmapCubicSplitMeshRenderer : MonoBehaviour
{
    public StarData starData;
    public Material material;

    public float distance = 100;
    public AnimationCurve sizeByMagnitudeCurve = new AnimationCurve(
        new Keyframe(0, 4f),
        new Keyframe(7, 1f));
    public bool magnitudeAffectsAlpha = true;
    public bool cullBelowHorizon = false;

    Star[] stars;
    List<Star> starsList;

    void Start()
    {
        if (!starData)
        {
            Debug.LogError("No StarData asset assigned to the renderer", this);

            enabled = false;
            return;
        }

        stars = starData.stars;

        if (cullBelowHorizon)
            starsList = CullBelowHorizon();
        else
            starsList = new List<Star>(stars);

        var cubicSplitStars = CubicSplit2(starsList);

        foreach (var cs in cubicSplitStars)
        {
            if (cs.Count == 0) continue;

            Mesh m = GenerateStarsAsStaticQuadsMesh(cs.ToArray());

            GameObject go = new GameObject("Stars");
            go.AddComponent<MeshFilter>().sharedMesh = m;
            go.AddComponent<MeshRenderer>().material = material;
            go.transform.SetParent(gameObject.transform, false);
        }
    }

    List<Star> CullBelowHorizon()
    {
        Star[] stars = starData.stars;
        List<Star> starsList = new List<Star>();

        for (int i = 0; i < stars.Length; i++)
        {
            Vector3 pos = stars[i].position;
            if (cullBelowHorizon &&
                transform.TransformDirection(pos).y > 0)
                starsList.Add(stars[i]);
        }

        return starsList;
    }

    List<Star>[] CubicSplit(List<Star> stars)
    {
        List<Star>[] cs = new List<Star>[6];

        for (int i = 0; i < cs.Length; i++)
            cs[i] = new List<Star>();

        float t = 1 / Mathf.Sqrt(2);

        for (int i = 0; i < stars.Count; i++)
        {
            Vector3 pos = stars[i].position.normalized;

            if (pos.x > -t && pos.x < t &&
                pos.y > -t && pos.y < t) // front or back
            {
                if (pos.z > 0) // front
                    cs[0].Add(stars[i]);
                else // back
                    cs[1].Add(stars[i]);
            }
            else if (pos.x > -t && pos.x < t &&
                    pos.z > -t && pos.z < t) // top or bottom
            {
                if (pos.y > 0) // top
                    cs[2].Add(stars[i]);
                else // bottom
                    cs[3].Add(stars[i]);
            }
            else // right or left
            {
                if (pos.x > 0) // right
                    cs[4].Add(stars[i]);
                else // left
                    cs[5].Add(stars[i]);
            }
        }

        return cs;
    }

    List<Star>[] CubicSplit2(List<Star> stars)
    {
        List<Star>[] cs = new List<Star>[6];

        for (int i = 0; i < cs.Length; i++)
            cs[i] = new List<Star>();

        float t = 1 / Mathf.Sqrt(2);

        for (int i = 0; i < stars.Count; i++)
        {
            int index = GetCubeMapIndex(stars[i].position);
            cs[index].Add(stars[i]);
        }

        return cs;
    }

    int GetCubeMapIndex(Vector3 pos)
    {
        float absX = Mathf.Abs(pos.x);
        float absY = Mathf.Abs(pos.y);
        float absZ = Mathf.Abs(pos.z);

        bool isXPositive = pos.x > 0;
        bool isYPositive = pos.y > 0;
        bool isZPositive = pos.z > 0;

        if (isXPositive && absX >= absY && absX >= absZ)
            return 0;
        else if (!isXPositive && absX >= absY && absX >= absZ)
            return 1;
        else if (isYPositive && absY >= absX && absY >= absZ)
            return 2;
        else if (!isYPositive && absY >= absX && absY >= absZ)
            return 3;
        else if (isZPositive && absZ >= absX && absZ >= absY)
            return 4;
        else if (!isZPositive && absZ >= absX && absZ >= absY)
            return 5;
        else
            return 0;
    }

    Mesh GenerateStarsAsStaticQuadsMesh(Star[] stars)
    {
        Mesh m = new Mesh();

        int vertexCount = stars.Length * 4;
        Vector3[] vertices = new Vector3[vertexCount];
        Color[] colors = new Color[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];

        int[] triangles = new int[stars.Length * 6];

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 0);
        Vector2 uv2 = new Vector2(0, 1);
        Vector2 uv3 = new Vector2(1, 1);

        for (int i = 0; i < stars.Length; i++)
        {
            Vector3 v = stars[i].position.normalized * distance;
            Vector3 dir = stars[i].position.normalized;

            Vector3 up = Vector3.ProjectOnPlane(Vector3.up, dir).normalized;
            Vector3 rt = Vector3.Cross(up, dir);

            float size = sizeByMagnitudeCurve.Evaluate(stars[i].magnitude);
            up *= 0.5f * size;
            rt *= 0.5f * size;

            int i0 = i * 4 + 0;
            int i1 = i * 4 + 1;
            int i2 = i * 4 + 2;
            int i3 = i * 4 + 3;

            vertices[i0] = v - rt - up;
            vertices[i1] = v + rt - up;
            vertices[i2] = v - rt + up;
            vertices[i3] = v + rt + up;

            uvs[i0] = uv0;
            uvs[i1] = uv1;
            uvs[i2] = uv2;
            uvs[i3] = uv3;

            Color c = Starmap.GetColorFromColorIndex(stars[i].colorIndex);
            if (magnitudeAffectsAlpha)
                c.a = Starmap.GetScaleFromMagnitude(stars[i].magnitude);
            colors[i0] = colors[i1] = colors[i2] = colors[i3] = c;

            triangles[i * 6 + 0] = i0;
            triangles[i * 6 + 1] = i2;
            triangles[i * 6 + 2] = i1;
            triangles[i * 6 + 3] = i1;
            triangles[i * 6 + 4] = i2;
            triangles[i * 6 + 5] = i3;
        }

        m.vertices = vertices;
        m.colors = colors;
        m.uv = uvs;
        m.triangles = triangles;

        m.RecalculateBounds();

        //m.bounds = new Bounds(
        //new Vector3(0, 0, 0),
        //new Vector3(distance * 2, distance * 2, distance * 2));

        return m;
    }
}
