using UnityEngine;

public class SnowBall : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float minRotateSpeedSqr = 0.0001f;

    private Vector2 _velocity;
    private float _damage;
    private GameObject _owner;
    private float _gravityScale;

    public void Initialize(Vector2 direction, float speed, float dmg, float gravScale, GameObject thrower)
    {
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;

        _velocity = direction.normalized * speed;
        _damage = dmg;
        _gravityScale = gravScale;
        _owner = thrower;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        _velocity.y -= gravity * _gravityScale * Time.deltaTime;
        transform.Translate(_velocity * Time.deltaTime, Space.World);

        if (_velocity.sqrMagnitude >= minRotateSpeedSqr)
        {
            float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _owner) return;

        CharacterHealth health = other.GetComponent<CharacterHealth>();
        if (health != null)
        {
            health.TakeDamage(_damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
