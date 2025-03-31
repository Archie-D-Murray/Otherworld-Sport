using System;

using Tags;

using UnityEngine;
using UnityEngine.InputSystem;

using Utilities;

public enum EndCause { Stamina, Floor }

public enum FireType { Bow, Laser, Railgun }

public class PlayerController : MonoBehaviour {
    [SerializeField] private float _maxFallSpeed = 5.0f;
    [SerializeField] private float _quickTimeTransitionSpeed = 10.0f;
    [SerializeField] private float _quickTimeSpeed = 0.5f;
    [SerializeField] private float _horizontalSpeed = 5.0f;
    [SerializeField] private float _updraftDuration = 10.0f;

    [SerializeField] private float _stamina = 5.0f;
    [SerializeField] private float _maxStamina = 5.0f;
    [SerializeField] private float _quickTimeDrain = 1.0f;
    [SerializeField] private float _shotDrain = 0.5f;

    [SerializeField] private float _fallSpeed = 0.0f;
    [SerializeField] private float _updraftSpeed = 0.0f;
    [SerializeField] private bool _falling = false;

    [SerializeField] private WeaponController _controller;
    [SerializeField] private FireType _type = FireType.Bow;

    [SerializeField] private BowController _bow;
    [SerializeField] private LaserController _laser;
    [SerializeField] private RailgunController _railgun;

    private CountDownTimer _fireTimer = new CountDownTimer(0.0f);
    private Rigidbody2D _rb2D;

    public CountDownTimer FireTimer => _fireTimer;

    public float StaminaPercent => _stamina / _maxStamina * UpgradeManager.Instance.StaminaMultiplier;

    public Action<EndCause> RunEnded;

    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
        _bow = GetComponent<BowController>();
        _laser = GetComponent<LaserController>();
        _railgun = GetComponent<RailgunController>();
        _bow.Init(this);
        _laser.Init(this);
        _railgun.Init(this);
        SetFireType(FireType.Railgun, true);
        PlayerInputs.Instance.FireAction.started += DrawStart;
        PlayerInputs.Instance.FireAction.canceled += DrawRelease;
    }

    private void FixedUpdate() {
        if (!_falling) { return; }
        if (PlayerInputs.Instance.QuickTime) {
            StaminaDrain(_quickTimeDrain * Time.fixedDeltaTime / UpgradeManager.Instance.StaminaMultiplier);
        }
        _fireTimer.Update(Time.fixedDeltaTime);
        _fallSpeed = Mathf.MoveTowards(_fallSpeed, TargetFallSpeed(), _quickTimeTransitionSpeed * Time.fixedDeltaTime);
        _rb2D.velocity = new Vector2(PlayerInputs.Instance.Horizontal * _horizontalSpeed, _updraftSpeed - _fallSpeed);
        _updraftSpeed = Mathf.MoveTowards(_updraftSpeed, 0.0f, Time.fixedDeltaTime * _updraftDuration);
    }

    private void DrawStart(InputAction.CallbackContext context) {
        if (!_falling) { return; }
        _controller.FirePress();
    }

    private void Update() {
        if (!_falling) { return; }
        _controller.FireHold(Time.deltaTime);
    }

    private void DrawRelease(InputAction.CallbackContext context) {
        if (!_falling) { return; }
        _controller.FireRelease();
        StaminaDrain(_shotDrain);
    }

    public void StaminaDrain(float amount) {
        if (_stamina <= 0.0f) {
            return;
        }
        _stamina -= amount;
        if (_stamina <= 0.0f) {
            RunEnded?.Invoke(EndCause.Stamina);
        }
    }

    public void Reset(Vector3 position) {
        _falling = false;
        _rb2D.velocity = Vector2.zero;
        transform.position = position;
        _stamina = _maxStamina * UpgradeManager.Instance.StaminaMultiplier;
    }

    public void Drop() {
        _falling = true;
        _rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private float TargetFallSpeed() {
        return PlayerInputs.Instance.QuickTime ? _quickTimeSpeed : _maxFallSpeed;
    }

    public void AddUpForce(float amount) {
        _updraftSpeed = amount;
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.HasComponent<Offscreen>()) {
            RunEnded?.Invoke(EndCause.Floor);
            RunEnd();
        }
    }

    private void RunEnd() {
        _rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _rb2D.velocity = Vector2.zero;
        _controller.FireRelease(true);
        _falling = false;
    }

    private void SetFireType(FireType type, bool force = false) {
        if (_type == type && !force) { return; }
        if (_controller) {
            _controller.FireRelease();
            _controller.Toggle(false);
        }
        switch (type) {
            case FireType.Railgun:
                _controller = _railgun;
                break;
            case FireType.Laser:
                _controller = _laser;
                break;
            default:
                _controller = _bow;
                break;
        }
        _controller.Toggle(true);
        _type = type;
    }
}