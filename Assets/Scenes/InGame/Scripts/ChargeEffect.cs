using UnityEngine;

public class ChargeEffect : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem normalChargeParticle;
    [SerializeField] private ParticleSystem fullChargeParticle;

    [Header("Burst Settings")]
    [SerializeField] private int particlesPerBurst = 5;
    [SerializeField] private int fullChargeBurst = 15;

    public void SpawnChargeParticle(bool isFullCharge)
    {
        if (isFullCharge)
        {
            if (fullChargeParticle != null)
            {
                fullChargeParticle.Emit(fullChargeBurst);
            }
        }
        else
        {
            if (normalChargeParticle != null)
            {
                normalChargeParticle.Emit(particlesPerBurst);
            }
        }
    }

    public void StopAllParticles()
    {
        if (normalChargeParticle != null)
        {
            normalChargeParticle.Clear();
        }

        if (fullChargeParticle != null)
        {
            fullChargeParticle.Clear();
        }
    }
}