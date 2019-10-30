using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditController : MonoBehaviour
{
    private void OnDisable()
    {
        playCredit = false;
    }
    private void OnEnable()
    {
        playCredit = true;
        Credits.position = startPos;
    }
    public bool playCredit;
    public RectTransform Credits;
    public float speed;
    public Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = Credits.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            speed *= 4;
        }

        if (Input.GetMouseButtonUp(0))
        {
            speed /= 4;
        }

        if (playCredit)
        {
            Credits.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        }
        Credits.anchoredPosition = new Vector2(0, Credits.anchoredPosition.y);
    }
}
