using UnityEngine;

public class SnowBallThrower : MonoBehaviour
{
    [Header("Snowball Settings")]
    [SerializeField] private GameObject snowballPrefab;

    [Header("Speed By Charge")]
    [SerializeField] private float speedNoCharge = 8f;
    [SerializeField] private float speedFullCharge = 16f;

    [Header("Damage By Charge")]
    [SerializeField] private float baseDamage = 5f;
    [SerializeField] private float maxDamage = 10f;

    [Header("Charge Settings")]
    [SerializeField] private float chargeInterval = 0.5f;
    [SerializeField] private int maxChargeLevel = 5;

    [Header("Gravity By Charge")]
    [SerializeField] private float gravityScaleNoCharge = 1f;
    [SerializeField] private float gravityScaleFullCharge = 0f;

    [Header("Trajectory Preview")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;
    [SerializeField] private float gravity = 9.8f;

    [Header("References")]
    [SerializeField] private ChargeEffect chargeEffect;
    [SerializeField] private Animator animator;

    private int _currentChargeLevel;
    private float _chargeTimer;
    private bool _isCharging;
    private Camera _mainCamera;

    public bool IsCharging => _isCharging;
    public int CurrentChargeLevel => _currentChargeLevel;

    private static readonly int AnimThrow = Animator.StringToHash("Throw");

    private void Start()
    {
        _mainCamera = Camera.main;

        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = trajectoryPoints;
            trajectoryLine.enabled = false;
        }
    }

    private void Update()
    {
        HandleCharging();

        if (_isCharging)
        {
            UpdateTrajectoryPreview();
        }
    }

    private void HandleCharging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCharging();
        }

        if (Input.GetMouseButton(0) && _isCharging)
        {
            _chargeTimer += Time.deltaTime;

            if (_chargeTimer >= chargeInterval && _currentChargeLevel < maxChargeLevel)
            {
                _chargeTimer = 0f;
                _currentChargeLevel++;

                if (chargeEffect != null)
                {
                    bool isFull = _currentChargeLevel >= maxChargeLevel;
                    chargeEffect.SpawnChargeParticle(isFull);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            ThrowSnowball();
        }
    }

    private void StartCharging()
    {
        _isCharging = true;
        _currentChargeLevel = 0;
        _chargeTimer = 0f;

        if (trajectoryLine != null) trajectoryLine.enabled = true;
    }

    public void CancelCharge()
    {
        _isCharging = false;
        _currentChargeLevel = 0;
        _chargeTimer = 0f;

        if (trajectoryLine != null) trajectoryLine.enabled = false;
        if (chargeEffect != null) chargeEffect.StopAllParticles();
    }

    private void ThrowSnowball()
    {
        if (animator != null) animator.SetTrigger(AnimThrow);
        if (snowballPrefab == null) { CancelCharge(); return; }

        Vector2 dir = GetAimDirection();
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.left;

        float chargeRatio = GetChargeRatio();

        float speed = Mathf.Lerp(speedNoCharge, speedFullCharge, chargeRatio);
        float damage = Mathf.Lerp(baseDamage, maxDamage, chargeRatio);
        float gravityScale = Mathf.Lerp(gravityScaleNoCharge, gravityScaleFullCharge, chargeRatio);

        GameObject snowball = Instantiate(snowballPrefab, transform.position, Quaternion.identity);
        SnowBall sb = snowball.GetComponent<SnowBall>();
        if (sb != null)
        {
            sb.Initialize(dir, speed, damage, gravityScale, gameObject);
        }

        _isCharging = false;
        _currentChargeLevel = 0;

        if (trajectoryLine != null) trajectoryLine.enabled = false;
        if (chargeEffect != null) chargeEffect.StopAllParticles();
    }

    private float GetChargeRatio()
    {
        if (maxChargeLevel <= 0) return 0f;
        return Mathf.Clamp01((float)_currentChargeLevel / maxChargeLevel);
    }

    private Vector2 GetAimDirection()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_mainCamera == null) return Vector2.left;

        Vector3 mouse = Input.mousePosition;
        Vector3 world = _mainCamera.ScreenToWorldPoint(mouse);
        world.z = transform.position.z;

        Vector2 delta = (Vector2)(world - transform.position);
        return delta.normalized;
    }

    private void UpdateTrajectoryPreview()
    {
        if (trajectoryLine == null) return;

        Vector2 dir = GetAimDirection();
        float chargeRatio = GetChargeRatio();

        float speed = Mathf.Lerp(speedNoCharge, speedFullCharge, chargeRatio);
        float gravityScale = Mathf.Lerp(gravityScaleNoCharge, gravityScaleFullCharge, chargeRatio);

        Vector2 vel = dir * speed;
        Vector3 pos = transform.position;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            trajectoryLine.SetPosition(i, pos);

            vel.y -= gravity * gravityScale * trajectoryTimeStep;
            pos += (Vector3)(vel * trajectoryTimeStep);
        }
    }
}
