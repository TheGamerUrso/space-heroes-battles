using UnityEngine;
namespace TheGamerUrso.Core
{
    /// <summary>
    /// Serves as the global entry point for the application.
    /// Guarantees that the persistent core prefab (_Game) is instantiated 
    /// across all scenes without requiring a dedicated Boot scene in the editor.
    /// </summary>
    public class GameBootstrapper
    {
        private const string GAME_PREFAB_RESOURCE_PATH = "_Game";
        private static bool isInitialized;
    

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeCoreServices()
        {
            if (isInitialized) return;
            isInitialized = true;

            // Check if _Game is already present in the active scene (active or inactive)
            GameObject existingGameObj = GameObject.Find(GAME_PREFAB_RESOURCE_PATH);
            if (existingGameObj != null)
            {
                isInitialized = true;
                Object.DontDestroyOnLoad(existingGameObj);
                return;
            }

            // Load and instantiate from Resources
            GameObject gamePrefab = Resources.Load<GameObject>(GAME_PREFAB_RESOURCE_PATH);
            if (gamePrefab == null)
            {
                Debug.LogError($"[{nameof(GameBootstrapper)}] Missing resource! Failed to load prefab at path: 'Resources/{GAME_PREFAB_RESOURCE_PATH}'");
                return;
            }

            GameObject gameInstance = Object.Instantiate(gamePrefab);
            gameInstance.name = GAME_PREFAB_RESOURCE_PATH;
            Object.DontDestroyOnLoad(gameInstance);

            //REGISTER ANY SERVICES HERE
        
            isInitialized = true;
            Debug.Log($"[{nameof(GameBootstrapper)}] Persistent '{GAME_PREFAB_RESOURCE_PATH}' successfully booted.");
        }
    }
}