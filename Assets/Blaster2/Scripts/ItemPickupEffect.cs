using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupEffect : MonoBehaviour
{
    public PlayerShip user;
    public RectTransform rectTransform;
    public float size;
    public float timer;
    public Image iconImage;
    public bool IsActive;
    public Sprite[] sprites;
    public float speed= 2;

    public void Show(int ItemIndex)
   {
        iconImage.sprite = sprites[ItemIndex];
        IsActive = true;
        size = 0;
        iconImage.gameObject.SetActive(true);
   }

    void Update()
    {
        if(!IsActive)return;
        size+=speed * Time.deltaTime;
        rectTransform.sizeDelta = new Vector2(size, size);
        if(size>25)
        {
            iconImage.gameObject.SetActive(false);
            size = 0;
            IsActive = false;
        }
    }
}
