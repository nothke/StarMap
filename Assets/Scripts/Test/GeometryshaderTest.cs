using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryshaderTest : MonoBehaviour
{
    public Material material;

    private void Start()
    {
        Mesh m = new Mesh();

        int vertNum = 3;

        Vector3[] vertices = new Vector3[vertNum];
        //Color[] colors = new Color[3];
        //int triLength = vertNum + (3 - vertNum % 3);
        //int[] triangles = new int[triLength];

        vertices[0] = Vector3.zero;
        vertices[1] = Vector3.right;
        vertices[2] = Vector3.right * 2;

        int[] triangles = new int[]
        {
            0, 0, 0,
            1, 1, 1,
            2, 2, 2
        };

        /*for (int i = 0; i < vertNum; i++)
        {
            vertices[i] = stars[i].position.normalized * 100;
            //colors[i] = new Color(0, 0, 0, stars[i].magnitude);
            triangles[i] = i;
        }*/

        m.vertices = vertices;
        //m.colors = colors;
        m.triangles = triangles;

        gameObject.AddComponent<MeshFilter>().sharedMesh = m;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }
}
