using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDetailViewListener : UIView
{
    public ScreenManager screenManager;
    public string id;
    private void Awake()
    {
        screenManager = GameObject.FindObjectOfType<ScreenManager>();
        screenManager.OnScreenChanged += OnScreenChangeCallback;
    }

    public void OnScreenChangeCallback(string id, bool active)
    {
        if (this.id.Equals(id))
        {
            if (active)
            {
                Open();
            }
            else if (!active)
            {
                Close();
            }
        }
    }
}
