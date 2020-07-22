using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneProgress : MonoBehaviour
{
    public Image progress;

    void Update()
    {
        progress.fillAmount = GameManager.Instance.sceneLoadProgress;
    }
}
