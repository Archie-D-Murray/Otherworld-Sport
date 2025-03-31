using UnityEngine;

using Tags;

public class RailgunController : WeaponController {
    [SerializeField] private Vector2 _fullCharge = Vector2.one;
    [SerializeField] private Vector2 _noCharge = Vector2.up;
    [SerializeField] private Color _chargedColour;
    [SerializeField] private bool _charged = false;

    private Transform _launchPoint;
    private SpriteRenderer _beamIndicator;
    [SerializeField] private SpriteRenderer _renderer;

    private void Start() {
        Transform railgun = GetComponentInChildren<Railgun>().transform;
        _renderer = railgun.GetComponent<SpriteRenderer>();
        _beamIndicator = railgun.GetChild(0).GetComponent<SpriteRenderer>();
        _launchPoint = railgun.parent;
        enabled = false;
    }

    private void OnDisable() {
        _launchPoint.OrNull()?.gameObject.SetActive(false);
    }

    private void OnEnable() {
        _launchPoint.OrNull()?.gameObject.SetActive(true);
    }

    public override void FireHold(float dt) {
        if (!_hold) { return; }
        if (!_charged) {
            _power = Mathf.MoveTowards(_power, maxPower, ChargeSpeed(dt));
            _chargeProgress = Mathf.InverseLerp(minPower, maxPower, _power);
            _beamIndicator.color = Color.Lerp(Color.white, Color.red, DrawProgress());
            _beamIndicator.size = Vector2.Lerp(_noCharge, _fullCharge, DrawProgress());
            if (_power == maxPower) {
                _charged = true;
                _beamIndicator.color = _chargedColour;
            }
        }
    }

    private void FixedUpdate() {
        _launchPoint.localRotation = Quaternion.RotateTowards(_launchPoint.localRotation, MouseRotation(), Time.fixedDeltaTime * _turnSpeed);
        _renderer.flipX = _launchPoint.localRotation.eulerAngles.z > 180.0f;
    }

    public override void FirePress() {
        if (!_player.FireTimer.IsFinished) { return; }
        _launchPoint.localRotation = Quaternion.RotateTowards(_launchPoint.localRotation, MouseRotation(), Time.deltaTime * _turnSpeed);
        _beamIndicator.color = Color.white;
        _beamIndicator.size = _noCharge;
        _power = minPower;
        _chargeProgress = 0.0f;
        _hold = true;
    }

    public override void FireRelease(bool destroy) {
        if (!_hold) { return; }
        if (_charged) {
            Instantiate(_projectile, _beamIndicator.transform.position, _beamIndicator.transform.rotation).GetComponent<Projectile>().Init(maxPower);
        }
        _beamIndicator.color = Color.white;
        _beamIndicator.size = _noCharge;
        _power = minPower;
        _chargeProgress = 0.0f;
        _charged = false;
        _hold = false;
        _player.FireTimer.Reset(_fireCooldown / UpgradeManager.Instance.DrawMultiplier);
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