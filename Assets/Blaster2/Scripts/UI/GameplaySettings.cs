using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettings : MonoBehaviour
{
    private PlayerData playerData;
    public Color SelectedColor;
    public Color DefaultColor;

    public Image[] Icons;
    private IDataService dataService;


    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
    }

    void Start()
    {
        playerData = dataService.GetPlayerData();

        for (int i = 0; i < Icons.Length; i++)
        {
            Icons[i].color = DefaultColor;
        }

        Icons[playerData.ControlScene - 1].color = SelectedColor;
    }

    public void UpdateGameplay(int index)
    {
        playerData.SetControlSceme(index);

        for (int i = 0; i < Icons.Length; i++)
        {
            Icons[i].color = DefaultColor;
        }

        Icons[index - 1].color = SelectedColor;
    }
}
