using UnityEngine;

public class FloatingTextTest : MonoBehaviour
{
    public FloatingText floatingText;

    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.localPosition);

        if (Input.GetKeyDown(KeyCode.F))
        {
            floatingText.gameObject.SetActive(true);
       
            floatingText.ShowFloatingText("Test", transform.position);
        }

        //Debug.DrawLine(transform.position, pos,Color.red);
    }
}
