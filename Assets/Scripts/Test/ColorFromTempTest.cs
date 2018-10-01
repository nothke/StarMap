using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StarMap;

public class ColorFromTempTest : MonoBehaviour
{
    [Range(0, 15000)]
    public float temperature = 1000;

    Camera _camera;
    new Camera camera { get { if (!_camera) _camera = GetComponent<Camera>(); return _camera; } }

    void Update()
    {
        camera.backgroundColor = Starmap.GetColorFromTemperature(temperature);
    }
}
