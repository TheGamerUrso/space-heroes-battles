using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class FollowMousePosition : MonoBehaviour
{
    public RectTransform rectTransform;
    public float speed;

    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    private float point;

    private float vertical;
    private float horizontal;
    private Touch currentTouch;
    private TouchPhase currentTouchPhase;
    private Vector3 newPos;
    public void Conroller()
    {
        vertical = CrossPlatformInputManager.GetAxis("Vertical");
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

        transform.position = new Vector3(horizontal, 0, vertical);
        newPos = transform.position;
        newPos.x += horizontal * speed * Time.deltaTime;
        newPos.z += vertical * speed * Time.deltaTime;
        transform.position = newPos;
    }

    void Update()
    {


        if (Input.touchCount > 0)
        {
            currentTouch = Input.GetTouch(0);
            currentTouchPhase = currentTouch.phase;
            if (currentTouchPhase == TouchPhase.Moved)
            {
                transform.position = currentTouch.position;
            }

            if (currentTouchPhase == TouchPhase.Stationary)
            {
                //ray = Camera.main.ScreenPointToRay(currentTouch.position);
                //point = 0f;
            }
        }



        transform.position = Input.mousePosition;

        rectTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(rectTransform.anchoredPosition.x, xMin, xMax),
            Mathf.Clamp(rectTransform.anchoredPosition.y, yMin, yMax));

    }
}
