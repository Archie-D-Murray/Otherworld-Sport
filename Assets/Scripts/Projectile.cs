using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    private Rigidbody2D _rb2D;
    [SerializeField] private float _timer = 3.0f;
    [SerializeField] private bool _fired = false;

    public void Init(float speed) {
        _rb2D = GetComponent<Rigidbody2D>();
        _rb2D.velocity = transform.up * speed;
        _fired = true;
    }

    private void FixedUpdate() {
        if (!_fired) { return; }
        _timer -= Time.fixedDeltaTime;
        if (_timer < 0.0f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        _rb2D.velocity = Vector2.zero;
    }
}