using UnityEngine;

[DisallowMultipleComponent]
public class AutoDisable : MonoBehaviour
{
    public float TTL;

    private void OnEnable()
    {
        Invoke("DisableGameObject", TTL);
    }

    void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}