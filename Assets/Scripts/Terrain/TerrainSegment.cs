using System;

using UnityEngine;

namespace Terrain {
    public class TerrainSegment : MonoBehaviour {

        [SerializeField] private float _height;

        public float Height() {
            return _height;
        }
    }
}