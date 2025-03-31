using System.Collections.Generic;

using UnityEngine;

namespace Terrain {
    public class Target : MonoBehaviour {
        [SerializeField] private BoxCollider2D _bullseye;
        [SerializeField] private BoxCollider2D _normal;
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] private int _value = 100;
        const int BULLSEYE_MULTIPLIER = 2;

        private HashSet<GameObject> _hit = new HashSet<GameObject>();

        protected virtual void OnTriggerEnter2D(Collider2D collider) {
            if (_hit.Contains(collider.gameObject)) { return; }
            _hit.Add(collider.gameObject);
            if (collider.IsTouching(_bullseye)) {
                Debug.Log("Bullseye!");
                UpgradeManager.Instance.Money += _value * BULLSEYE_MULTIPLIER;
                SpawnHit(true, _value * BULLSEYE_MULTIPLIER);
            } else {
                Debug.Log("Normal.");
                UpgradeManager.Instance.Money += _value;
                SpawnHit(false, _value);
            }
            TerrainManager.Instance.DestroyTarget(this);
        }

        public virtual void SetType(TerrainData data) {
            _renderer.sprite = data.TargetSprite;
        }

        protected virtual void SpawnHit(bool bullseye, int value) {
            TerrainManager.Instance.SpawnHitMessage(bullseye, value, transform.position);
        }
    }
}