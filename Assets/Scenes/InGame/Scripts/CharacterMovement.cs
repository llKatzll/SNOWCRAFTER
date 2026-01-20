using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isPlayer = true;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float moveInput;

    private static readonly int AnimIsMoving = Animator.StringToHash("_isMoving");

    private void Update()
    {
        //first. moving 구현부터 
        if (isPlayer)
        {
            moveInput = 0f;
            if (Input.GetKey(KeyCode.A)) moveInput = -1f;
            if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        }

        if (moveInput != 0f)
        {
            transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime, Space.World);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = (moveInput > 0f);
            }
        }

        if (animator != null)
        {
            animator.SetBool(AnimIsMoving, moveInput != 0f);
        }
    }
}
