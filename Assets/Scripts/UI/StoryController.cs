using System.Collections;
using TMPro;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    public GameObject StoryCanvas;
    public TextMeshProUGUI textMeshProUGUI;

    [Multiline]
    public string[] storystrings;

    public int currentStoryToPlay;
    public bool KeyPressed;

    private void Start()
    {
        StartCoroutine(StoryCoroutine());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            KeyPressed = true;
        }
    }

    private IEnumerator StoryCoroutine()
    {
        while (currentStoryToPlay < storystrings.Length)
        {
            string textToShow = "";
            char[] charArray = storystrings[currentStoryToPlay].ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i].Equals('@'))
                {
                    int numb = int.Parse("" + charArray[i + 1]);
                    DiscussionManager.Instance.StartDiscussion(numb);
                }

                if (charArray[i].Equals('%'))
                {
                    SceneLoader.Instance.LoadScene("Level" + "" + charArray[i]);
                }

                textToShow += "" + charArray[i];
            }

            while (DiscussionManager.Instance.IsPlayingDiscussion())
            {
                StoryCanvas.SetActive(false);
                yield return new WaitForSeconds(1);
            }

            textMeshProUGUI.text = textToShow;

            StoryCanvas.SetActive(true);

            while (KeyPressed == false)
            {
                yield return null;
            }

            currentStoryToPlay++;

            yield return new WaitForSeconds(1);

            if (currentStoryToPlay >= storystrings.Length)
            {
                currentStoryToPlay = storystrings.Length;
            }

            KeyPressed = false;
        }
    }
}