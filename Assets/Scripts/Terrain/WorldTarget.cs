using System;
using System.Collections.Generic;

using UnityEngine;

namespace Terrain {
    public class WorldTarget : Target {

        static readonly TerrainType[] Types = (TerrainType[])Enum.GetValues(typeof(TerrainType));
        static readonly int TypeCount = Enum.GetValues(typeof(TerrainType)).Length - 1;

        [SerializeField] private TerrainType _nextWorld;
        [SerializeField] private SpriteRenderer _outline;

        private void Start() {
            List<TerrainType> available = new List<TerrainType>(TypeCount);
            foreach (TerrainType type in Types) {
                if (type == TerrainManager.Instance.WorldType) { continue; }
                available.Add(type);
            }
            _nextWorld = available.GetRandomValue();
            _outline.color = TerrainManager.Instance.GetAccent(_nextWorld);
        }

        protected override void OnTriggerEnter2D(Collider2D collider) {
            TerrainManager.Instance.SetWorldType(_nextWorld);
            base.OnTriggerEnter2D(collider);
        }
    }
}