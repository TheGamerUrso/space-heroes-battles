using TMPro;
using UnityEngine;

public class ComboKillIndicator : MonoBehaviour
{
    private PlayerShip playerShip;
    public GameObject Window;
    public TextMeshProUGUI MultiplierText;

    public Animator animator;
    private float timerCooldown = 1;

    private void OnDestroy()
    {
        Events.PlayerShipHit -= ZeroMiltiplier;
        Events.OnMultiplierChanged -= OnMultiplierChanged;
    }

    private void Start()
    {
        HideWindow();
        Events.PlayerShipHit += ZeroMiltiplier;
        Events.OnMultiplierChanged += OnMultiplierChanged;
    }

    public void ShowWindow()
    {
        Window.SetActive(true);
    }

    public void HideWindow()
    {
        Window.SetActive(false);
    }

    public void OnMultiplierChanged(int multiplier)
    {

        if (!PlayerPrefs.HasKey("KillMultiTut"))
        {
            Tutorial.Instance.ShowTutorial(6);
            PlayerPrefs.SetInt("KillMultiTut", 1);
        }

        if (timerCooldown <= 0)
        {
            animator.SetTrigger("KillConfirm");

            ShowWindow();
            RefreshText(multiplier);
        }



    }

    public void Update()
    {
        if (timerCooldown > 0)
        {
            timerCooldown -= Time.deltaTime;
        }
    }

    public void ZeroMiltiplier()
    {
        HideWindow();
    }

    public void RefreshText(int multiplier)
    {
        MultiplierText.text = "x" + multiplier;
    }

}