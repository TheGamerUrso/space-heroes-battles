
using System.Collections;
using UnityEngine;

public class HitShieldEffect : MonoBehaviour
{

    public bool playing;
    public MeshRenderer meshRenderer;
    public float alpha;
    public Color startingColor;
    public Color color;
    public bool FadeIn;
    public bool FadeOut;

    private void OnEnable()
    {
        if (!playing)
            StartCoroutine(HitEffect());
    }
    private void Awake()
    {
        startingColor = meshRenderer.material.GetColor("_BaseColor");
        transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (FadeIn)
        {
            alpha += Time.deltaTime;
            color.a = alpha;
            meshRenderer.material.SetColor("_BaseColor", color);
            if (alpha >= startingColor.a)
            {
                FadeIn = false;
            }
        }

        if (FadeOut)
        {
            alpha -= Time.deltaTime;
            color.a = alpha;
            meshRenderer.material.SetColor("_BaseColor", color);
            if (alpha <= 0)
            {
                FadeOut = false;
            }
        }
    }

    IEnumerator HitEffect()
    {
    
        color = startingColor;
        alpha = color.a;

        playing = true;
        FadeIn = true;

        while (alpha > startingColor.a)
        {
            yield return null;
        }

        yield return new WaitForSeconds(.25f);

        FadeOut = true;

        if (alpha <= 0)
        {
            yield return null;
        }
        playing = false;
        transform.parent.gameObject.SetActive(false);
    }
}
