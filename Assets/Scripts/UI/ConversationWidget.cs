using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationWidget : MonoBehaviour
{
    public TextMeshProUGUI StoryText;
    private string Text;
    private string ActualTextShowned;
    public float speed;
    private int curIndex;
    private bool skip;
    public ContentSizeFitter contentSizeFitter;

    private void OnEnable()
    {
        StartCoroutine(AnimateText(Text));
        ActualTextShowned = string.Empty;
        StoryText.text = ActualTextShowned;
        contentSizeFitter.enabled = false;
    }

    public void SetStory(string text)
    {
        skip = false;
        Text = text;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            skip = true;
        }
    }

    private IEnumerator AnimateText(string text)
    {
        char[] words = text.ToCharArray();
        curIndex = 0;

        while (skip == false && curIndex <= words.Length)
        {
            yield return new WaitForSeconds(speed);
            ActualTextShowned += words[curIndex];
            StoryText.text = ActualTextShowned;
            curIndex++;
            if(!contentSizeFitter.enabled)
            contentSizeFitter.enabled = true;
        }

        curIndex = words.Length;
        StoryText.text = text;
    }
}