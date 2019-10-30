using UnityEngine;
using UnityEngine.UI;

public class UIFollowTarget : MonoBehaviour
{

    public GameObject Target;
    public GameObject RotateTarget;

    public Vector3 offset;

    void Awake()
    {
        GetComponent<Image>().enabled = false;
        Invoke("EnableImage", 1);
    }

    void Update()
    {
        GetComponent<RectTransform>().position =
            Camera.main.WorldToScreenPoint(Target.transform.position) + offset;
    }

    void EnableImage()
    {
        GetComponent<Image>().enabled = true;
    }
}
