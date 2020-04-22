using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct Discussion
{
    public int ID;
    public string Name;
    public string Text;
}

[Serializable]
public class DiscussionQue
{
    public Discussion[] discussions;
}

public class DiscussionManager : MonoBehaviour
{
    public static DiscussionManager Instance;

    public List<DiscussionQue> ListOfDiscussions = new List<DiscussionQue>();
    public int currentIndex;
    public GameObject DiscutionScreen;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DiscText;
    private string textToShow;
    private bool NextDisc = true;
    private int CurrentDiscIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    public void StartDiscussion(int index)
    {
        StartCoroutine(PlayDiscussion(index));
    }

    private void Update()
    {
        if (DiscutionScreen.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                currentIndex++;
                NextDisc = true;
            }
        }
    }

    public bool IsPlayingDiscussion()
    {
        if (DiscutionScreen.activeSelf)
        {
            return true;
        }
        return false;
    }

    public void GetNextDiscussion()
    {
    }

    private IEnumerator PlayDiscussion(int discIndex)
    {
        CurrentDiscIndex = discIndex;
        DiscutionScreen.SetActive(true);
        currentIndex = 0;
        while (currentIndex < (ListOfDiscussions[discIndex].discussions.Length))
        {
            NameText.text = ListOfDiscussions[discIndex].discussions[currentIndex].Name;
            Char[] wordList = ListOfDiscussions[discIndex].discussions[currentIndex].Text.ToCharArray();
            if (NextDisc)
            {
                textToShow = "";
                for (int i = 0; i < wordList.Length; i++)
                {
                    textToShow += wordList[i];
                    DiscText.text = textToShow;
                    if (DiscText.text.Length == wordList.Length)
                    {
                        break;
                    }
                }
            }
            NextDisc = false;
            yield return new WaitForSeconds(0.1f);
        }
        currentIndex = 0;
        DiscutionScreen.SetActive(false);
    }
}