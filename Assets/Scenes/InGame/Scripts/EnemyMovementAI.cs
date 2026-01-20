using UnityEngine;

public class EnemyMovementAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float minMoveInterval = 0.5f;
    [SerializeField] private float maxMoveInterval = 2f;
    [SerializeField] private float moveDuration = 0.3f;

    [Header("Boundaries")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = -1f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float _moveTimer;
    private float _moveEndTime;
    private float _moveDirection;
    private bool _isMoving;

    private static readonly int AnimIsMoving = Animator.StringToHash("_isMoving");

    private void Start()
    {
        ResetMoveTimer();
        _isMoving = false;
        _moveDirection = -1f;
        ApplyFacingInverted(_moveDirection);
        UpdateAnimation();
    }

    private void Update()
    {
        if (_isMoving) HandleMoving();
        else HandleIdle();

        UpdateAnimation();
    }

    private void HandleIdle()
    {
        _moveTimer -= Time.deltaTime;
        if (_moveTimer <= 0f) StartMoving();
    }

    private void StartMoving()
    {
        _isMoving = true;

        float dir = Random.value > 0.5f ? 1f : -1f;

        float x = transform.position.x;
        if (x <= minX + 0.01f) dir = 1f;
        else if (x >= maxX - 0.01f) dir = -1f;

        _moveDirection = dir;
        _moveEndTime = Time.time + moveDuration;

        ApplyFacingInverted(_moveDirection);
    }

    private void HandleMoving()
    {
        float newX = transform.position.x + _moveDirection * moveSpeed * Time.deltaTime;

        if (newX <= minX)
        {
            newX = minX;
            _moveDirection = 1f;
            ApplyFacingInverted(_moveDirection);
        }
        else if (newX >= maxX)
        {
            newX = maxX;
            _moveDirection = -1f;
            ApplyFacingInverted(_moveDirection);
        }

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        if (Time.time >= _moveEndTime)
        {
            _isMoving = false;
            ResetMoveTimer();
        }
    }

    private void ResetMoveTimer()
    {
        _moveTimer = Random.Range(minMoveInterval, maxMoveInterval);
    }

    // Inverted flip rule:
    // dir > 0 means moving right, but flipX becomes true (inverted)
    private void ApplyFacingInverted(float dir)
    {
        if (spriteRenderer == null) return;

        bool movingRight = dir > 0f;
        spriteRenderer.flipX = movingRight;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        animator.SetBool(AnimIsMoving, _isMoving);
    }
}
