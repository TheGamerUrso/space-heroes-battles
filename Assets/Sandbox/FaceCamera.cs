using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(Camera.main.transform.position, -transform.forward);
    }
}