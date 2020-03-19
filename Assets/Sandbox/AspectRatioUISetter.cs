using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(LayoutElement))]
public class AspectRatioUISetter : MonoBehaviour
{
    public bool refresh;
    public LayoutElement layoutElement;
    public float[] Aspects;
    public int currentAspect;



    void Start()
    {
        if (layoutElement == null)
            layoutElement = GetComponent<LayoutElement>();

        float aspect = Camera.main.aspect;
        Debug.Log(Camera.main.aspect);
        Debug.Log(aspect);

        if (aspect == .5f)
        {// 16:9{
            currentAspect = 0;
        }
        else if (aspect > .5f)
        {// 16:9

            currentAspect = 1;
            float diffrent = aspect - .5f;
            Aspects[1] += diffrent;

        }


        if (refresh)
        {
            layoutElement.preferredHeight = Aspects[currentAspect];
        }

        layoutElement.preferredHeight = Aspects[currentAspect];
    }

}
