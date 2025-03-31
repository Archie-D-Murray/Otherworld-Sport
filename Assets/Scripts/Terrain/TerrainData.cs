using UnityEngine;

namespace Terrain {
    [CreateAssetMenu(menuName = "Terrain")]
    public class TerrainData : ScriptableObject {
        public TerrainType Type;
        public Sprite Background;
        public Sprite TargetSprite;
        public Color TargetAccent;
    }
}