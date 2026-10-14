using PrimeTween;
using UnityEngine;

public class AboutToShootIndicator : MonoBehaviour
{
    public BaseWeapon baseWeapon;
    public bool playEffect;
    public float speed;
    public float size = 1;

    void Start()
    {
        baseWeapon.AboutToShoot += AboutToShoot;
    }

    public void AboutToShoot(bool shooting)
    {
        if (shooting)
        {
            if (!playEffect)
            {
                playEffect = true;
                Tween.Scale(transform,endValue:size,duration:1).OnComplete(() =>
                {     
                    transform.localScale = new Vector3(0, 0, 0);
                });
            }
        }
        else if (!shooting)
        {
            playEffect = false;
        }

    }
}
