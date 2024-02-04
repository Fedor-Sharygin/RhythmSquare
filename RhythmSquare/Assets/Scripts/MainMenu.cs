using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelManager;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown tdLevelSelect;
    [SerializeField]
    private MenuAnimationLoop malRef;
    public void Awake()
    {
        tdLevelSelect.ClearOptions();
        List<string> NewOptions = new List<string>();
        foreach (LevelInfo Level in LevelLoader.giGameInfo.Levels)
        {
            NewOptions.Add(Level.LevelName);
        }
        tdLevelSelect.AddOptions(NewOptions);

        int ClipIdx = Random.Range(0, LevelManager.LevelLoader.giGameInfo.Levels.Count);
        malRef.StartAudio(ClipIdx);
        tdLevelSelect.value = ClipIdx;
    }

    public void SelectLevel()
    {
        malRef.StartAudio(tdLevelSelect.value);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        //int RIdx = Random.Range(0, LevelManager.LevelLoader.giGameInfo.Levels.Count);
        LevelLoader.LoadLevel(tdLevelSelect.value);
    }
}
