using UnityEngine;

public class EnemySnowBallAI : MonoBehaviour
{
    [Header("Snowball Settings")]
    [SerializeField] private GameObject snowballPrefab;
    [SerializeField] private float throwSpeed = 18f;
    [SerializeField] private float damage = 8f;

    [Header("AI Cooldown")]
    [SerializeField] private float throwCooldownMin = 1f;
    [SerializeField] private float throwCooldownMax = 3f;

    [Header("Aim")]
    [SerializeField] private Transform aimTarget;

    [Header("References")]
    [SerializeField] private ChargeEffect chargeEffect;
    [SerializeField] private Animator animator;

    private float _cooldownTimer;

    public bool IsCharging => false;

    private static readonly int AnimThrow = Animator.StringToHash("Throw");

    private void Start()
    {
        ResetCooldown();
    }

    private void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
        {
            ThrowFastStraight();
            ResetCooldown();
        }
    }

    private void ThrowFastStraight()
    {
        if (animator != null) animator.SetTrigger(AnimThrow);
        if (snowballPrefab == null) return;

        Vector2 dir = GetAimDirection();
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        GameObject snowball = Instantiate(snowballPrefab, transform.position, Quaternion.identity);
        SnowBall sb = snowball.GetComponent<SnowBall>();
        if (sb != null)
        {
            sb.Initialize(dir, throwSpeed, damage, 0f, gameObject);
        }

        if (chargeEffect != null) chargeEffect.StopAllParticles();
    }

    private Vector2 GetAimDirection()
    {
        if (aimTarget == null) return Vector2.right;
        Vector2 delta = (Vector2)(aimTarget.position - transform.position);
        return delta.normalized;
    }

    private void ResetCooldown()
    {
        _cooldownTimer = Random.Range(throwCooldownMin, throwCooldownMax);
    }

    public void CancelCharge()
    {
        if (chargeEffect != null) chargeEffect.StopAllParticles();
        ResetCooldown();
    }

    public void SetPlayer(Transform playerTransform)
    {
        aimTarget = playerTransform;
    }
}
