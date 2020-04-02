using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;


    private PlayerShip playerShip;
    private void OnDestroy()
    {
        if (playerShip == null)
            playerShip = PlayerManager.GetPlayer();

        if (playerShip != null)
            playerShip.PlayerShipHit -= StartEffect;
    }
    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    public void StartEffect()
    {
        shakeDuration = .5f;

    }
    private void Start()
    {
      

    }

    void Update()
    {
        if (playerShip == null)
        {
            playerShip = PlayerManager.GetPlayer();

            if (playerShip != null)
                playerShip.PlayerShipHit += StartEffect;
        }

        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }
}