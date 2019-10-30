using TMPro;
using UnityEngine;

public class ComboKillIndicator : MonoBehaviour
{
    public GameObject Window;
    public TextMeshProUGUI MultiplierText;
    public int Multiplier;

    public Animator animator;
    private float timerCooldown = 1;
    public static ComboKillIndicator instance;

    private void Start()
    {
        HideWindow();
    }

    public void ShowWindow()
    {
        Window.SetActive(true);
    }

    public void HideWindow()
    {
        Window.SetActive(false);
    }

    public int GetMultiplier()
    {
        return Multiplier;
    }

    public void ConfirmKill()
    {
        if (!PlayerPrefs.HasKey("KillMultiTut"))
        {
            Tutorial.Instance.ShowTutorial(6);
            PlayerPrefs.SetInt("KillMultiTut", 1);
        }

        if (timerCooldown <= 0)
        {
            animator.SetTrigger("KillConfirm");
            if (this.Multiplier < 4)
            {
                this.Multiplier++;
            }
            ShowWindow();
            RefreshText();
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
        this.Multiplier = 0;
        RefreshText();
        HideWindow();
    }

    public void RefreshText()
    {
        MultiplierText.text = string.Format("x{0}", Multiplier);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}