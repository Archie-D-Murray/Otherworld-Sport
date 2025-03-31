using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    private Rigidbody2D _rb2D;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private float _timer = 3.0f;
    [SerializeField] private bool _fired = false;
    [SerializeField] private bool _destroy = true;

    private void Start() {
        if (!_rb2D) { _rb2D = GetComponent<Rigidbody2D>(); }
        if (!_collider2D) { _collider2D = GetComponent<Collider2D>(); }
    }

    public void Init(float speed, bool destroy = true) {
        if (!_rb2D) { _rb2D = GetComponent<Rigidbody2D>(); }
        if (!_collider2D) { _collider2D = GetComponent<Collider2D>(); }
        _rb2D.velocity = transform.up * speed;
        _fired = true;
        _collider2D.enabled = true;
        _destroy = destroy;
    }

    private void FixedUpdate() {
        if (!_fired) { return; }
        transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, _rb2D.velocity.normalized), Vector3.forward);
        _timer -= Time.fixedDeltaTime;
        if (_timer < 0.0f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (_destroy) {
            Destroy(gameObject);
        } else {
            _rb2D.gravityScale = 0.0f;
            _rb2D.velocity = Vector2.zero;
            transform.parent = collider.transform;
            _fired = false;
        }
    }
}