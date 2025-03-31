using UnityEngine;

public abstract class WeaponController : MonoBehaviour {
    [SerializeField] protected bool _hold = false;
    [SerializeField] protected float _fireCooldown = 1.0f;
    [SerializeField] protected float _maxUp = 22.5f;

    [SerializeField] protected float _minPower = 1.0f;
    [SerializeField] protected float _maxPower = 5.0f;
    [SerializeField] protected float _chargeSpeed = 4.0f;
    [SerializeField] protected float _chargeProgress = 0.0f;
    [SerializeField] protected float _turnSpeed = 45.0f;
    [SerializeField] protected float _power = 0.0f;
    [SerializeField] protected GameObject _projectile;

    [SerializeField] protected PlayerController _player;

    protected float minPower => _minPower * UpgradeManager.Instance.PowerMultiplier;
    protected float maxPower => _maxPower * UpgradeManager.Instance.PowerMultiplier;
    protected float ChargeSpeed(float dt) => ((_maxPower - _minPower) / _chargeSpeed) * dt * UpgradeManager.Instance.DrawMultiplier * UpgradeManager.Instance.PowerMultiplier;

    public virtual void Init(PlayerController player) {
        _player = player;
    }

    public float FireCooldown => _fireCooldown;
    public bool Pressed => _hold;

    public abstract void FirePress();
    public abstract void FireHold(float dt);
    public abstract void FireRelease(bool destroy = false);

    public void Toggle(bool on) {
        enabled = on;
    }
}