using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarMap
{
    [System.Serializable]
    public struct Star
    {
        public string name;
        public Vector3 position;
        public float magnitude;
        public float colorIndex;
    }

    [CreateAssetMenu(menuName = "StarData")]
    public class StarData : ScriptableObject
    {
        [SerializeField]
        public Star[] stars;

        public void LoadFromDatabase(float magnitudeLimit)
        {
            stars = Starmap.LoadFromDatabase(magnitudeLimit);
        }
    }
}
