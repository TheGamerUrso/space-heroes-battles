using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using Unity.Cinemachine;
using UnityEngine;

public class ShakeController : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shakeAmount = 0.7f;
    private IEventService eventService;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
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
        float forceMultiplier = Mathf.Clamp01(shakeAmount / duration);
        impulseSource.GenerateImpulseWithForce(forceMultiplier);
    }

    public void ShakeCameraEventHandled(ShakeCameraEvent payload)
    {
        Shake(payload.duration);
    }
}