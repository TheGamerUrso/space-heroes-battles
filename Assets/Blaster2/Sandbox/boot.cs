using UnityEngine;

/**
 * Automatically Run this beforeSceneLoad
 */
public class boot
{
    // Runs before a scene gets loaded
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadMain()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            gameManager = GameObject.Instantiate(Resources.Load("GameManager")) as GameManager;
            gameManager.name = "GameManager";
        }
    }
    // You can choose to add any "Service" component to the Main prefab.
    // Examples are: Input, Saving, Sound, Config, Asset Bundles, Advertisements
}