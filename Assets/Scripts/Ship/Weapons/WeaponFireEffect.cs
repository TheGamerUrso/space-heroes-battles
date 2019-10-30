using UnityEngine;

public class WeaponFireEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSFX;

    private void OnValidate()
    {
        particleSFX = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        if (particleSFX == null)
            particleSFX = GetComponentInChildren<ParticleSystem>();
    }

    public void PlayEffect()
    {
        particleSFX.Play();
    }
}