using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
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

    private IEventService eventService;

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
    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
        eventService.Subscribe<ShakeCameraEvent>(ShakeCameraEventHandled);
    }

    private void OnDestroy()
    {     
        eventService.Unsubscribe<ShakeCameraEvent>(ShakeCameraEventHandled);
    }
    public void Shake(float duration = .5f)
    {
        shakeDuration = duration;
    }

    public void ShakeCameraEventHandled(ShakeCameraEvent payload)
    {
        Shake(payload.duration);
    }


    void Update()
    {
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