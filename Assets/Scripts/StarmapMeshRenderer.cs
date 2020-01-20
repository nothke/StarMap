using UnityEngine;
using System.Collections.Generic;

namespace StarMap
{
    public class StarmapMeshRenderer : MonoBehaviour
    {
        public StarData starData;
        public Material material;

        [Tooltip("How far will the stars be rendered from the origin")]
        public float distance = 100;
        [Tooltip("0 are brightest stars and falling off with magnitude")]
        public AnimationCurve sizeByMagnitudeCurve = new AnimationCurve(
            new Keyframe(0, 4f),
            new Keyframe(7, 1f));
        [Tooltip("If true, magnitude will be saved into alpha, " +
            "so dimmer stars will be more transparent if using a shader " +
            "with vertex color such as a particle shader.")]
        public bool magnitudeAffectsAlpha = true;
        [Tooltip("Stars below the horizon will not be spawned. " +
            "If you intend to rotate the sky, like in case you have a time of day, " +
            "you should disable this.")]
        public bool cullBelowHorizon = false;
        [Tooltip("Since the stars exactly on the horizon could be visible, we want to have a little bit of margin. Only relevant if cullBelowHorizon is true.")]
        [Range(0, 1)]
        public float cullBelowHorizonOffset = 0.05f;
        [Tooltip("Cubic splitting will divide the stars into 6 separate meshes. This improves performance since only those sides that are visible are rendered (frustum culling). " +
            "You will probably want to have this option always on.")]
        public bool useCubicSplitting = true;

        void Start()
        {
            if (!starData)
            {
                Debug.LogError("No StarData asset assigned to the renderer", this);

                enabled = false;
                return;
            }

            Star[] stars = starData.stars;

            List<Star> starsList = cullBelowHorizon ?
                starsList = Starmap.GetStarsCulledBelowHorizon(stars, transform.InverseTransformDirection(Vector3.up), cullBelowHorizonOffset) :
                starsList = new List<Star>(stars);


            if (useCubicSplitting)
            {
                var cubicSplitStars = CubicSplit(starsList);

                foreach (var cs in cubicSplitStars)
                {
                    var splits = Starmap.SplitToMax(cs, Starmap.MESH_STAR_COUNT_LIMIT);

                    foreach (var chunk in splits)
                    {
                        Mesh m = GenerateStarsAsStaticQuadsMesh(chunk);

                        GameObject go = new GameObject("Stars");
                        go.AddComponent<MeshFilter>().sharedMesh = m;
                        go.AddComponent<MeshRenderer>().material = material;
                        go.transform.SetParent(gameObject.transform, false);
                    }
                }
            }
            else
            {
                var splits = Starmap.SplitToMax(starsList, Starmap.MESH_STAR_COUNT_LIMIT);

                foreach (var chunk in splits)
                {
                    Mesh m = GenerateStarsAsStaticQuadsMesh(chunk);

                    GameObject go = new GameObject("Stars");
                    go.AddComponent<MeshFilter>().sharedMesh = m;
                    go.AddComponent<MeshRenderer>().material = material;
                    go.transform.SetParent(gameObject.transform, false);
                }
            }
        }

        List<Star>[] CubicSplit(List<Star> stars)
        {
            List<Star>[] cs = new List<Star>[6];

            for (int i = 0; i < cs.Length; i++)
                cs[i] = new List<Star>();

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
}
