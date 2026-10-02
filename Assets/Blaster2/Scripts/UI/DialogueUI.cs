using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIView
{
    private event Action OnContinueButtonPressed;
    public Image potraitImg;
    public TextMeshProUGUI dialogueText;
    public DialogueManager dialogueManager;

    float delayInput = .5f;

    private void Start()
    {
        dialogueManager.OnNewDialogueQueued += DialogueManager_OnNewDialogueQueued;
    }

    private void Update()
    {
        if (!IsActive) return;

        delayInput -= Time.deltaTime;
        if (delayInput <= 0)
        {
            if (Input.anyKeyDown)
            {
                delayInput = .5f;
                OnContinueButtonPressed?.Invoke();
                Hide();
            }
        }

    }

    private void DialogueManager_OnNewDialogueQueued(
        Dialogue dialogue,Action callback)
    {
        Hide();
        OnContinueButtonPressed = callback;
        Setup(dialogue.potrait, dialogue.dialogue);
        Show();
    }

    public void Setup(Sprite potrait,string dialogue)
    {
        potraitImg.sprite = potrait;
        dialogueText.text = dialogue;
    }
}
