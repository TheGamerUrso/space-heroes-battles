using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public Sprite potrait;
    public string dialogue;
}
[Serializable]
public class Dialogues
{
    public List<Dialogue> dialogues;
}

public enum DialogueEnum
{
    NONE,PLAYING,PROCEED,STOPPED
}

public class DialogueManager : MonoBehaviour
{
    public event Action<Dialogue, Action> OnNewDialogueQueued;
    public event Action OnDialgoueFinished;
    public DialogueEnum currentDialogueState;

    public Queue<Dialogue> DialogueQue = new Queue<Dialogue>();

    [DictionaryDisplay]
    [SerializeField]
    public Dictionary<int, Dialogues> dialoguesList = new Dictionary<int, Dialogues>();

    private float timer = 1;

    private void Update()
    {
        switch (currentDialogueState)
        {
            case DialogueEnum.PLAYING:       
                break;
            case DialogueEnum.PROCEED:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {      
                    if (DialogueQue.Count > 0 )
                    {
                        var nextDialogue = DialogueQue.Dequeue();
                        OnNewDialogueQueued?.Invoke(nextDialogue, () =>
                        {
                            if (DialogueQue.Count > 0)
                            {
                                currentDialogueState = DialogueEnum.PROCEED;
                            }
                            else
                            {
                                currentDialogueState = DialogueEnum.STOPPED;
                                OnDialgoueFinished?.Invoke();
                            }
                        });
                        currentDialogueState = DialogueEnum.PLAYING;
                    }
                }
                break;
            case DialogueEnum.STOPPED:
                break;
        }
    }
    public bool ShowDialogue(int Id,Action callback)
    {
        OnDialgoueFinished = callback;
        var tempDialogues = dialoguesList[Id];
        if(dialoguesList.TryGetValue(Id,out Dialogues dialogues))
        {
            foreach (var dialogue in dialogues.dialogues)
            {
                DialogueQue.Enqueue(dialogue);
            }
            currentDialogueState = DialogueEnum.PROCEED;
            return true;
        }
        return false;
    }
}
