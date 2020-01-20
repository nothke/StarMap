using System.Collections.Generic;
using UnityEngine;

namespace StarMap
{
    public class StarmapGSRenderer : MonoBehaviour
    {
        public StarData starData;
        public Material material;

        public float distance = 100;
        public bool cullBelowHorizon = false;

        void Start()
        {
            if (!starData)
            {
                Debug.LogError("No StarData asset assigned to the renderer", this);

                enabled = false;
                return;
            }

            var stars = new List<Star>(starData.stars);

            var splits = Starmap.SplitToMax(stars, Starmap.MESH_STAR_COUNT_LIMIT);

            foreach (var chunk in splits)
            {
                Mesh m = GenerateStarsAsGeometryMesh(chunk);

                GameObject go = new GameObject("Stars");
                go.AddComponent<MeshFilter>().sharedMesh = m;
                go.AddComponent<MeshRenderer>().material = material;
                go.transform.SetParent(gameObject.transform, false);
            }

        }

        Mesh GenerateStarsAsGeometryMesh(Star[] stars)
        {
            Mesh m = new Mesh();

            Vector3[] vertices = new Vector3[stars.Length];
            Color[] colors = new Color[stars.Length];

            int[] triangles = new int[stars.Length * 3];

            for (int i = 0; i < stars.Length; i++)
            {
                /*if (cullBelowHorizon &&
                    transform.TransformDirection(stars[i].position).y < 0)
                    continue;*/

                vertices[i] = stars[i].position.normalized * distance;

                colors[i] = Starmap.GetColorFromColorIndex(stars[i].colorIndex);
                colors[i].a = Starmap.GetScaleFromMagnitude(stars[i].magnitude);

                triangles[i * 3] = i;
                triangles[i * 3 + 1] = i;
                triangles[i * 3 + 2] = i;
            }

            m.vertices = vertices;
            m.colors = colors;
            m.triangles = triangles;

            return m;
        }
    }
}
