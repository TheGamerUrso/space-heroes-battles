using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorldPositionHandler : MonoBehaviour
{
    public LayerMask mLayerMask;
    private float RotationSpeed = 25f;
    private Quaternion _lookRotation;
    private Vector3 _direction;

    public void Update()
    {
        //transform.RotateAround(transform.position, Vector3.up, ControlsManager.getAxisInput("Mouse X") * mouse_rotation_speed);
        //find the vector pointing from our position to the target
        _direction = (OnWorldPosition(mLayerMask) - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);
        _lookRotation.x = 0;
        _lookRotation.z = 0;
        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
    }

    //--------------------------------------------------
    public static GameObject MouseOnWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
       

        }
        return null;
    }

    //--------------------------------------------------
    public static Vector3 OnWorldPosition(LayerMask mLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, mLayerMask))
        {
            // print("I Hit World " + hit.collider.gameObject + " Point " + hit.point);
            return hit.point;
        }
        return Vector2.zero;

    }


    public void lookMouseDirection()
    {
        //transform.RotateAround(transform.position, Vector3.up, ControlsManager.getAxisInput("Mouse X") * mouse_rotation_speed);
        //find the vector pointing from our position to the target
        _direction = (OnWorldPosition(mLayerMask) - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);
        _lookRotation.x = 0;
        _lookRotation.z = 0;
        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
    }


    public void TurnOnMovingDirection(Vector3 direction)
    {
        var moveDirection = Camera.main.transform.TransformDirection(direction);
        moveDirection.y = 0;
        moveDirection *= Time.deltaTime;
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(moveDirection),
                    Time.deltaTime * RotationSpeed);
            }
        }
    }

}
