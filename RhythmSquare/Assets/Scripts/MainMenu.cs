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


    public static int iDropdownIdx { set; private get; }
    public void SelectLevel()
    {
        iDropdownIdx = tdLevelSelect.value;
    }
    private void Update()
    {
        if (tdLevelSelect.value != iDropdownIdx)
        {
            tdLevelSelect.value = iDropdownIdx;
        }
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
