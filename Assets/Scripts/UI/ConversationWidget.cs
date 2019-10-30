using System.Collections;
using TMPro;
using UnityEngine;

public class ConversationWidget : MonoBehaviour
{
    public TextMeshProUGUI StoryText;
    private string Text;
    private string ActualTextShowned;
    public float speed;
    private int curIndex;
    private bool skip;

    private void OnEnable()
    {
        StartCoroutine(AnimateText(Text));
        ActualTextShowned = string.Empty;
        StoryText.text = ActualTextShowned;
        // StoryText.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, -1578, 0);
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
        }

        curIndex = words.Length;
        StoryText.text = text;
    }
}