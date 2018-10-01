using UnityEngine;

public class StarmapMeshRenderer : MonoBehaviour
{
    public StarData starData;
    public Material material;
    
    public float distance = 100;
    public AnimationCurve sizeByMagnitudeCurve = new AnimationCurve(
        new Keyframe(0, 4f),
        new Keyframe(7, 1f));
    public bool magnitudeAffectsAlpha = true;

    Star[] stars;

    void Start()
    {
        if (!starData)
        {
            Debug.LogError("No StarData asset assigned to the renderer", this);

            enabled = false;
            return;
        }

        stars = starData.stars;

        Mesh m = GenerateStarsAsStaticQuadsMesh();

        gameObject.AddComponent<MeshFilter>().sharedMesh = m;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }

    Mesh GenerateStarsAsStaticQuadsMesh()
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

        m.bounds = new Bounds(
            new Vector3(0, 0, 0),
            new Vector3(distance * 2, distance * 2, distance * 2));

        return m;
    }
}
