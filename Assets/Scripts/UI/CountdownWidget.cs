using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownWidget : SystemMessageWidget
{
	public TextMeshProUGUI m_Countdown;
	private float previousNum = 0;

	[SerializeField] private float m_Size = 1;
	[SerializeField] private float m_Speed = 1;

	void Start () {
		m_Size = 0;
	}

	void Update () {

		if (m_Canvas.activeSelf) {
			m_Size += m_Speed * Time.deltaTime;
			m_Countdown.transform.localScale = new Vector3 (m_Size, m_Size, m_Size);
        }

        if (m_Size > 1f)
        {
           // Enabled(false);
        }
	}

	public void UpdateCountdownText(float number){
		if (number != previousNum) {
			m_Countdown.transform.localScale = Vector3.zero;
            SetWidgetText(string.Format("{0}", number));
			m_Size = 0;
			previousNum = number;
		}
	}

}
