using TMPro;
using UnityEngine;
using DG.Tweening;

public class ComboKillIndicator : MonoBehaviour
{
    public readonly string[] congratulations = { "Not Bad", "Nice!", "Good!", "Great!", "Supser","Suberb!", "Perfect!", "Godlike!" };

    [SerializeField]private GameObject TextCanvas;
    [SerializeField]private TextMeshProUGUI confratulationText;


    private float TTL = 1;
    private float timerCooldown = 1;
    private int perfect;

    private float duration = 1;
    private int vibriate = 5;
    private int elasticity = 1;
    private Ease ease = Ease.InOutQuart;
    private Vector3 size = new Vector3(.25f, .25f, .25f);

    private void OnDestroy()
    {
        Events.OnMultiplierChanged -= OnMultiplierChanged;
    }

    private void Start()
    {
        HideText();
        Events.OnMultiplierChanged += OnMultiplierChanged;
    }

    public void ShowText()
    {
        Animate();
        TextCanvas.SetActive(true);
    }

    public void HideText()
    {
        TextCanvas.SetActive(false);
    }


    public void Animate()
    {
        confratulationText.transform.DOPunchScale(size, duration, vibriate, elasticity).SetEase(ease);
    }

    public void OnMultiplierChanged()
    {
        perfect++;

        if (Game.Multiplier == 0)
        {
            perfect = 0;
        }

        if (timerCooldown <= 0 && TextCanvas.activeInHierarchy == false)
        {
            ShowText();
            GenerateText();
        }
    }

    public void Update()
    {
        if (!TextCanvas.activeInHierarchy)
        {
            timerCooldown -= Time.deltaTime;
            if (timerCooldown <= 0)
            {
                timerCooldown = 0;
            }
        }
        else if (TextCanvas.activeInHierarchy)
        {
            TTL -= Time.deltaTime;
            if (TTL <= 0)
            {
                TTL = 2;
                HideText();
            }
        }
    }

    public void GenerateText()
    {
        string finalText = string.Empty;
        int num = 0;

        if (perfect > 30)
        {
            num = Random.Range(5, congratulations.Length);
            finalText = congratulations[num];
        }
        else if (perfect > 20)
        {
            num = Random.Range(3, 5);
            finalText = congratulations[num];
        }
        else if (perfect > 10)
        {
            num = Random.Range(1, 3);
            finalText = congratulations[num];
        }
        else if (perfect >= 5)
        {
            finalText = congratulations[num];
        }

        confratulationText.text = finalText;

        timerCooldown = Random.Range(2, 4);
    }

}