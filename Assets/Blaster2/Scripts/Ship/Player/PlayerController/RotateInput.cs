using System;
using UnityEngine;

[Serializable]
public class RotateInput
{
    private float dist;
    private float distFromCenter;
    public Vector2 worldToViewport;
    public Quaternion GetRotateByCenter(Transform transform, Vector3 targetPos, Vector3 newTargetRotation)
    {
        worldToViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        bool turn = (worldToViewport.x <= .1f || worldToViewport.x >=.9);
        if (turn)
        {
            dist = (transform.position - targetPos).magnitude;

            distFromCenter = (transform.position - new Vector3(0, -50, transform.position.z)).magnitude;

            distFromCenter = Mathf.Clamp(distFromCenter, -10, 10);

            if (transform.position.x < 0)
            {
                distFromCenter *= -1;
            }

            newTargetRotation.y = distFromCenter;

        }
        return Quaternion.Euler(newTargetRotation);
    }

    public static Quaternion TiltRotationTowardsVelocity(Quaternion cleanRotation, Vector3 referenceUp, Vector3 vel, float velMagFor45Degree)
    {
        Vector3 rotAxis = Vector3.Cross(referenceUp, vel);
        float tiltAngle = Mathf.Atan(vel.magnitude / velMagFor45Degree) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(tiltAngle, rotAxis) * cleanRotation;    //order matters
    }
}