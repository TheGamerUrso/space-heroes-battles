using UnityEngine;

public class MoveShipIntro : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        GameObject.FindObjectOfType<IntroScreen>().onIntroClickContinue += () => { animator.SetTrigger("Go"); };
    }
     
}
