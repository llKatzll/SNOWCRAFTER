using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private CharacterHealth target;
    [SerializeField] private Image fillImage;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFill = 1f;

    private void Start()
    {
        if (target != null)
        {
            target.OnHealthChanged += UpdateHealth;
        }
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            target.OnHealthChanged -= UpdateHealth;
        }
    }

    private void Update()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, smoothSpeed * Time.deltaTime);
        }
    }

    private void UpdateHealth(float current, float max)
    {
        targetFill = current / max;
    }
}