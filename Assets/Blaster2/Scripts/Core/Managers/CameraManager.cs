using TheGamerUrso.Core;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : ServiceComponent<ICameraService>,ICameraService
{
   [SerializeField] private CinemachineCamera cinemachineCamera;
    public void SetTarget(GameObject target)
    {
        cinemachineCamera.Follow =  target.transform;
    }
}
