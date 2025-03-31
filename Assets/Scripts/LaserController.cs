using UnityEngine;

using Tags;

public class LaserController : WeaponController {
    private Projectile _laser;
    private Transform _launchPoint;
    private ParticleSystem _particles;

    private void Start() {
        _launchPoint = GetComponentInChildren<Laser>().transform.parent;
        _particles = _launchPoint.GetComponentInChildren<ParticleSystem>();
        _particles.Stop();
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
        _power = Mathf.MoveTowards(_power, maxPower, ChargeSpeed(dt));
        _chargeProgress = Mathf.InverseLerp(minPower, maxPower, _power);
        GetEmission();
    }

    private void FixedUpdate() {
        _launchPoint.localRotation = Quaternion.RotateTowards(_launchPoint.localRotation, MouseRotation(), Time.fixedDeltaTime * _turnSpeed);
    }

    public override void FirePress() {
        if (!_player.FireTimer.IsFinished) { return; }
        _particles.Play();
        _launchPoint.localRotation = MouseRotation();
        GetEmission();
        _power = minPower;
        _chargeProgress = 0.0f;
        _laser = Instantiate(_projectile, _launchPoint.position, _launchPoint.localRotation).GetComponent<Projectile>();
        _hold = true;
    }

    public override void FireRelease(bool destroy) {
        if (!_hold) { return; }
        if (destroy) {
            Destroy(_laser.gameObject);
        } else {
            _laser.Init(_power);
        }
        _laser = null;
        GetEmission();
        _power = minPower;
        _chargeProgress = 0.0f;
        _hold = false;
        _player.FireTimer.Reset(_fireCooldown / UpgradeManager.Instance.DrawMultiplier);
        _particles.Stop();
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

    private void GetEmission() {
        ParticleSystem.EmissionModule emission = _particles.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(_chargeProgress * 100.0f);
    }
}