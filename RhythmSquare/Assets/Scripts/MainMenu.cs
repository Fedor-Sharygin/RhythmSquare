using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static void Quit()
    {
        Application.Quit();
    }

    public static void Play()
    {
        int RIdx = Random.Range(0, LevelManager.LevelLoader.giGameInfo.Levels.Count);
        LevelManager.LevelLoader.LoadLevel(RIdx);
    }
}
