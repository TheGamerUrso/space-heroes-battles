using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
public class LevelDetailViewListener : MonoBehaviour
{
    public ScreenManager screenManager;
    
    public string id;
    UIView uIView;

    private void Awake()
    {
        screenManager = GameObject.FindObjectOfType<ScreenManager>();
        screenManager.OnScreenChanged += OnScreenChangeCallback;
        uIView = GetComponent<UIView>();
    }

    public void OnScreenChangeCallback(string id, bool active)
    {
        if (this.id.Equals(id))
        {
            if (active)
            {
                uIView.Show();
            }
            else if (!active)
            {
                uIView.Hide();
            }
        }
    }
}
