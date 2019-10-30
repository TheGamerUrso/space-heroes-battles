using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Reward
{
    public Sprite m_Sprite;
    public string m_Name;
}

public class WaveRewardWidget : MonoBehaviour
{
    //public GameObject m_RewardHolder;
    //public GameObject m_RewardElementPrefab;
    //[SerializeField]
    //public List<Reward> m_Rewards = new List<Reward>();
    //public List<RewardElement> m_RewardElements;

    //public RewardElement m_RewardElement;

    //public bool RewardChoosen = false;


    //public void InitializeReward()
    //{
    //    gameObject.SetActive(true);
    //    m_RewardElements = new List<RewardElement>();
    //    List<Reward> m_AvailableRewards = m_Rewards.ToList<Reward>();
    //    for (int i = 0; i < 3; i++)
    //    {
    //        GameObject reward = Instantiate(m_RewardElementPrefab, m_RewardHolder.transform, false);

    //        int chancePresent = UnityEngine.Random.Range(0, 100);
    //        int chance = 0;
    //        if (chancePresent <= 15)
    //        {
    //            chance = 2;
    //            foreach (RewardElement item in m_RewardElements)
    //            {
    //                if (item.nameOfReward.text == "Shield")
    //                {
    //                    chance = 0;
    //                }
    //            }
    //        }
    //        else if (chancePresent > 15 && chancePresent <= 25)
    //        {
    //            chance = 1;
    //            foreach (RewardElement item in m_RewardElements)
    //            {
    //                if (item.nameOfReward.text == "Power")
    //                {
    //                    chance = 0;
    //                }
    //            }
    //        }
    //        else if (chancePresent > 25)
    //        {
    //            chance = 0;
    //        }
    //        Reward rwd = m_AvailableRewards[chance];
    //        int rewardValue = 0;
    //        if (rwd.m_Name.Equals("Coins"))
    //        {
    //            rewardValue = UnityEngine.Random.Range(5, 15);
    //        }

    //        reward.GetComponent<RewardElement>().Initialize(rwd.m_Sprite, rwd.m_Name, rewardValue);
    //        m_RewardElements.Add(reward.GetComponent<RewardElement>());
    //    }
    //}

    //public void Clear()
    //{
    //    foreach (RewardElement item in m_RewardElements)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //}

    //void Update()
    //{
    //    foreach (RewardElement item in m_RewardElements)
    //    {
    //        if (!item.m_HiddenImage.enabled && !RewardChoosen)
    //        {
    //            RewardChoosen = true;
    //            StartCoroutine(RewardChosen());
    //        }
    //        else if (item.m_HiddenImage.enabled && RewardChoosen)
    //        {
    //            item.m_HiddenImage.enabled = false;
    //            item.nameOfReward.enabled = true;
    //        }

    //        if (item.m_Highlight.enabled && m_RewardElement == null)
    //        {
    //            m_RewardElement = item;
    //        }
    //    }
    //}


    //IEnumerator RewardChosen()
    //{
    //    yield return new WaitForSeconds(1);
    //    //Game.Instance.ChangeGameState(GameStateType.Game);
    //    RewardChoosen = false;
    //    Clear();
    //    Player mPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    //    switch (m_RewardElement.nameOfReward.text)
    //    {
    //        case "Power":
    //            mPlayer.GetComponent<PlayerWeaponSystem>().IncreasePowerUp(1);
    //            Debug.Log(m_RewardElement.nameOfReward.text);
    //            break;
    //        case "Coins":
    //            GameManager.instance.EarnCoins(m_RewardElement.Reward);
    //            Debug.Log(m_RewardElement.nameOfReward.text);
    //            break;
    //        case "Shield":
    //            PlayerWidget playerWidget = GameObject.FindObjectOfType<PlayerWidget>();

    //            if (!playerWidget.m_ShieldImage.IsActive())
    //            {
    //                mPlayer.ShieldUp();
    //            }

    //            Debug.Log(m_RewardElement.nameOfReward.text);
    //            break;
    //        default:
    //            break;
    //    }
    //    gameObject.SetActive(false);
    //    // EnemyManager.Instance.state = EnemyManager.SpawnState.WAITING;
    //    Cursor.visible = false;

    //}
}
