using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {
    public LevelEnum nextScene;
	void Start () 
    {
        SceneLoader.LoadScene(nextScene);
    }
}
