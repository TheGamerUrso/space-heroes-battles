using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDialog : UIView
{
    public TextMeshProUGUI dialogMessageText;

    public virtual void OK()
    {
        Hide();
    }
    IEnumerator CloseWithDelay()
    {
        yield return new WaitForSeconds(1.0f);
        Hide();
    }

    public void SetText(string score)
    {
        dialogMessageText.text = score;
    }
}
