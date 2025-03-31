using UnityEngine;

using Tags;

public class LaserController : WeaponController {
    private Projectile _laser;
    private Transform _pivot;
    private ParticleSystem _particles;
    private SpriteRenderer _renderer;

    private void Start() {
        Transform laser = GetComponentInChildren<Laser>().transform;
        _pivot = laser.parent;
        _renderer = laser.GetComponent<SpriteRenderer>();
        _particles = _pivot.GetComponentInChildren<ParticleSystem>();
        _particles.Stop();
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
        _laser.transform.SetPositionAndRotation(_particles.transform.position, _pivot.rotation);
        GetEmission();
    }

    private void FixedUpdate() {
        _pivot.localRotation = Quaternion.RotateTowards(_pivot.localRotation, MouseRotation(), Time.fixedDeltaTime * _turnSpeed);
        _renderer.flipX = _pivot.localRotation.eulerAngles.z < 180.0f;
    }

    public override void FirePress() {
        if (!_player.FireTimer.IsFinished) { return; }
        _particles.Play();
        _pivot.localRotation = MouseRotation();
        GetEmission();
        _power = minPower;
        _chargeProgress = 0.0f;
        _laser = Instantiate(_projectile, _particles.transform.position, _particles.transform.localRotation).GetComponent<Projectile>();
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
        _power = minPower;
        _chargeProgress = 0.0f;
        GetEmission();
        _hold = false;
        _player.FireTimer.Reset(_fireCooldown / UpgradeManager.Instance.DrawMultiplier);
        _particles.Stop();
    }

    public float DrawProgress() {
        return _chargeProgress;
    }

    private Quaternion MouseRotation() {
        float angle = PlayerInputs.Instance.MouseAngle(_pivot.position);
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