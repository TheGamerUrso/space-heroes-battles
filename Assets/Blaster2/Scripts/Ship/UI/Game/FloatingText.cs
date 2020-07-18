using UnityEngine;
using TMPro;
public class FloatingText : MonoBehaviour
{
    public float TTL;
    public TextMeshProUGUI Text;
    public Transform Target;

    public Transform FloatingTextPivot ;
    public RectTransform TextRectTransform;
    private Camera Cam;

    public Vector3 offset;
    private Vector3 worldToScreenPos;
    private Vector3 targetPos;
    private void Start()
    {
        Cam = Camera.main;
    }

    private void OnEnable()
    {
        TTL = 2;
    }

    private void OnDisable()
    {
        gameObject.transform.position = Vector3.zero;
    }

    public void ShowFloatingText(string text, Vector3 pos)
    {
        targetPos = pos;
        Text.text = text;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos);
        FloatingTextPivot.transform.position = screenPos + offset;
    }



    void Update()
    {



        if (TTL > 0)
        {
            TTL -= Time.deltaTime;
        }

        if (TTL <= 0)
        {
            gameObject.SetActive(false);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPos, 2);
    }
}
