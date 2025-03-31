using UnityEngine;

namespace Terrain {
    public class Updraft : MonoBehaviour {
        [SerializeField] private float _minForce = 2.0f;
        [SerializeField] private float _maxForce = 5.0f;
        [SerializeField] private float _distance = 4.0f;

        private float _top;

        private void Start() {
            _distance = GetComponent<BoxCollider2D>().size.y;
        }

        private void OnTriggerStay2D(Collider2D collider) {
            if (!collider.TryGetComponent(out PlayerController controller)) { return; }
            float distance = collider.transform.position.y - transform.position.y;
            controller.AddUpForce(Mathf.Lerp(_maxForce, _minForce, distance / _distance));
        }
    }
}