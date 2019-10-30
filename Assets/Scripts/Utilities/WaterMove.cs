using UnityEngine;

public class WaterMove : MonoBehaviour
{
    public Material waterMat;

    public float SPEED;

    private void Update()
    {
        Vector4 POS = new Vector4(0, 0, 0, 0);
        POS.y += SPEED * Time.deltaTime;
        Vector4 waterSpeed = waterMat.GetVector("WaveSpeed");
        waterSpeed += (Vector4)Vector3.forward;
    }
}