using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragRotate : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject RotateTarget;
    public bool AutoRotate;
    public float RotateSpeed;
    private float ReEnableAutoRotateTimer = 1f;

    void Update() 
    {
        if (AutoRotate)
        {
            RotateTarget.transform.Rotate(0, RotateSpeed * Time.deltaTime, 0);
        }

        if(AutoRotate == false && ReEnableAutoRotateTimer > 0)
        {
            ReEnableAutoRotateTimer -= Time.deltaTime;
            if(ReEnableAutoRotateTimer < 0)
            {
                AutoRotate = true;
            }

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        AutoRotate = false;
        GetComponent<Button>().interactable = false;
        float clampedRotateSpeed = Mathf.Clamp(Input.GetAxis("Mouse X"),-RotateSpeed, RotateSpeed);
        RotateTarget.transform.Rotate(new Vector3(0, clampedRotateSpeed, 0));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Button>().interactable = true;
        ReEnableAutoRotateTimer = 1;
    }
}
