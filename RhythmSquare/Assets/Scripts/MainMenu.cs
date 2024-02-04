using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelManager;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown tdLevelSelect;
    public void Awake()
    {
        tdLevelSelect.ClearOptions();
        List<string> NewOptions = new List<string>();
        foreach (LevelInfo Level in LevelLoader.giGameInfo.Levels)
        {
            NewOptions.Add(Level.LevelName);
        }
        tdLevelSelect.AddOptions(NewOptions);
    }

    public static void SelectLevel()
    {

    }

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
