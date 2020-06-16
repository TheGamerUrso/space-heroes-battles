using UnityEngine;

public class Planet : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Texture[] materials;
    public float speed;

    private void OnEnable()
    {
        meshRenderer.material.SetTexture("TextureMap", materials[UnityEngine.Random.Range(0, materials.Length)]);
    }

    private void Update()
    {
        transform.position -= transform.forward * speed * Time.deltaTime;

    }

    private void LateUpdate()
    {
        if (transform.localPosition.z < -256)
        {
            gameObject.SetActive(false);
        }
    }
}