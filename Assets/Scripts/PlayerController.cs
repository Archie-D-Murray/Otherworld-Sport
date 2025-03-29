using Tags;

using UnityEngine;
using UnityEngine.InputSystem;

using Utilities;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float _maxFallSpeed = 5.0f;
    [SerializeField] private float _quickTimeTransitionSpeed = 10.0f;
    [SerializeField] private float _quickTimeSpeed = 0.5f;

    [SerializeField] private float _stamina = 5.0f;
    [SerializeField] private float _quickTimeDrain = 1.0f;
    [SerializeField] private float _shotDrain = 0.5f;
    [SerializeField] private float _idleDrain = 0.05f;
    [SerializeField] private float _drainMultiplier = 1.0f;

    [SerializeField] private float _minPower = 1.0f;
    [SerializeField] private float _maxPower = 5.0f;
    [SerializeField] private float _chargeSpeed = 4.0f;
    [SerializeField] private float _chargeProgress = 0.0f;
    [SerializeField] private float _turnSpeed = 45.0f;
    [SerializeField] private bool _charging = false;

    [SerializeField] private float _maxUp = 22.5f;

    [SerializeField] private float _fallSpeed = 0.0f;
    [SerializeField] private float _power = 0.0f;
    [SerializeField] private float _fireCooldown = 1.0f;
    [SerializeField] private bool _quickTime = false;

    [SerializeField] private GameObject _projectile;

    private CountDownTimer _fireTimer = new CountDownTimer(0.0f);
    private Transform _launchPoint;
    private Transform _fullDraw;
    private Transform _noDraw;
    private SpriteRenderer _bowRenderer;
    private Animator _bowAnimator;
    private Rigidbody2D _rb2D;
    private readonly int _drawID = Animator.StringToHash("Draw");
    private readonly int _fireID = Animator.StringToHash("Fire");
    private Projectile _bow;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
        _launchPoint = transform.GetChild(0);
        Debug.Log($"Launch point: {_launchPoint.name}");
        _bowRenderer = _launchPoint.GetComponentInChildren<SpriteRenderer>();
        _bowAnimator = _launchPoint.GetComponentInChildren<Animator>();
        _noDraw = _bowAnimator.transform.GetChild(0);
        _fullDraw = _bowAnimator.transform.GetChild(1);

        PlayerInputs.Instance.FireAction.started += DrawStart;
        PlayerInputs.Instance.FireAction.canceled += DrawRelease;
    }

    private void FixedUpdate() {
        _fireTimer.Update(Time.fixedDeltaTime);
        _launchPoint.localRotation = Quaternion.RotateTowards(_launchPoint.localRotation, MouseRotation(), Time.fixedDeltaTime * _turnSpeed);
        _fallSpeed = Mathf.MoveTowards(_fallSpeed, TargetFallSpeed(), _quickTimeTransitionSpeed * Time.fixedDeltaTime);
        _rb2D.velocity = Vector2.down * _fallSpeed;
    }

    private void DrawStart(InputAction.CallbackContext context) {
        if (!_fireTimer.IsFinished) { return; }
        _launchPoint.localRotation = MouseRotation();
        _bowRenderer.color = Color.white;
        _power = _minPower;
        _bowAnimator.Play(_drawID);
        _bowAnimator.speed = _chargeSpeed / (_maxPower - _minPower);
        _chargeProgress = 0.0f;
        _bow = Instantiate(_projectile, _noDraw.position, _launchPoint.localRotation).GetComponent<Projectile>();
        _charging = true;
    }

    private void Update() {
        if (!_charging) { return; }
        _power = Mathf.MoveTowards(_power, _maxPower, _chargeSpeed * Time.deltaTime);
        _chargeProgress = Mathf.InverseLerp(_minPower, _maxPower, _power);
        _bowRenderer.color = Color.Lerp(Color.white, Color.red, DrawProgress());
        _bow.transform.SetPositionAndRotation(Vector3.Lerp(_noDraw.position, _fullDraw.position, Mathf.Sqrt(_chargeProgress)), _launchPoint.localRotation);
    }

    private void DrawRelease(InputAction.CallbackContext context) {
        if (!_charging) { return; }
        _bow.Init(_power);
        _bow = null;
        _bowRenderer.color = Color.white;
        _power = _minPower;
        _bowAnimator.speed = 1.0f;
        _bowAnimator.Play(_fireID);
        _chargeProgress = 0.0f;
        _charging = false;
        _fireTimer.Reset(_fireCooldown);
    }

    private float TargetFallSpeed() {
        return PlayerInputs.Instance.QuickTime ? _quickTimeSpeed : _maxFallSpeed;
    }

    public float DrawProgress() {
        return _chargeProgress;
    }

    private Quaternion MouseRotation() {
        float angle = PlayerInputs.Instance.MouseAngle(_launchPoint.position);
        if (angle > 0.0f) {
            angle = Mathf.Clamp(angle, _maxUp, 180.0f);
        } else {
            angle = Mathf.Clamp(angle, -180.0f, -_maxUp);
        }
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}