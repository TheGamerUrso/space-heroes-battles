using UnityEngine;

public static class PlayerPrefsUtils
{
    public static void SaveBool(string id, int num)
    {
        PlayerPrefs.SetInt(id, num);
    }

    public static bool LoadBool(string id)
    {
        int num = PlayerPrefs.GetInt(id, 0);

        if (num == 0)
        {
            return false;
        }
        else if (num == 1)
        {
            return true;
        }
        return false;
    }
}