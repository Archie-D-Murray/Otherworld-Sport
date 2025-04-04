using UnityEngine;

using Tags;

public class BowController : WeaponController {

    private Projectile _arrow;
    private readonly int _drawID = Animator.StringToHash("Draw");
    private readonly int _fireID = Animator.StringToHash("Fire");
    private Transform _pivot;
    private Transform _fullDraw;
    private Transform _noDraw;
    private SpriteRenderer _bowRenderer;
    private Animator _bowAnimator;

    private void Start() {
        _pivot = GetComponentInChildren<Bow>().transform.parent;
        _bowRenderer = _pivot.GetComponentInChildren<SpriteRenderer>();
        _bowAnimator = _pivot.GetComponentInChildren<Animator>();
        _noDraw = _bowAnimator.transform.GetChild(0);
        _fullDraw = _bowAnimator.transform.GetChild(1);
        enabled = false;
    }

    private void OnDisable() {
        _pivot.OrNull()?.gameObject.SetActive(false);
    }

    private void OnEnable() {
        _pivot.OrNull()?.gameObject.SetActive(true);
    }

    public override void FireHold(float dt) {
        if (!_hold) { return; }
        _power = Mathf.MoveTowards(_power, maxPower, ChargeSpeed(dt));
        _chargeProgress = Mathf.InverseLerp(minPower, maxPower, _power);
        _bowRenderer.color = Color.Lerp(Color.white, Color.red, DrawProgress());
        _arrow.transform.SetPositionAndRotation(Vector3.Lerp(_noDraw.position, _fullDraw.position, Mathf.Sqrt(_chargeProgress)), _pivot.localRotation);
    }

    private void FixedUpdate() {
        if (_player.FireTimer.IsFinished && !_hold) {
            _bowAnimator.speed = 1.0f;
        }
        _pivot.localRotation = Quaternion.RotateTowards(_pivot.localRotation, MouseRotation(), Time.fixedDeltaTime * _turnSpeed);
    }

    public override void FirePress() {
        if (!_player.FireTimer.IsFinished) { return; }
        _pivot.localRotation = MouseRotation();
        _bowRenderer.color = Color.white;
        _power = minPower;
        _bowAnimator.Play(_drawID);
        _bowAnimator.speed = (maxPower - minPower) * UpgradeManager.Instance.DrawMultiplier / _chargeSpeed;
        _chargeProgress = 0.0f;
        _arrow = Instantiate(_projectile, _noDraw.position, _pivot.localRotation).GetComponent<Projectile>();
        _hold = true;
    }

    public override void FireRelease(bool destroy) {
        if (!_hold) { return; }
        if (destroy) {
            Destroy(_arrow.gameObject);
        } else {
            _arrow.Init(_power, false);
            _emitter.Play(SoundEffectType.Bow);
        }
        _arrow = null;
        _bowRenderer.color = Color.white;
        _power = minPower;
        _bowAnimator.speed = 1.0f;
        _bowAnimator.Play(_fireID);
        _chargeProgress = 0.0f;
        _hold = false;
        _player.FireTimer.Reset(_fireCooldown / UpgradeManager.Instance.DrawMultiplier);
        _bowAnimator.speed = UpgradeManager.Instance.DrawMultiplier;
    }

    public float DrawProgress() {
        return _chargeProgress;
    }

    private Quaternion MouseRotation() {
        float angle = PlayerInputs.Instance.MouseAngle(transform.position);
        if (angle > 0.0f) {
            angle = Mathf.Clamp(angle, _maxUp, 180.0f);
        } else {
            angle = Mathf.Clamp(angle, -180.0f, -_maxUp);
        }
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}