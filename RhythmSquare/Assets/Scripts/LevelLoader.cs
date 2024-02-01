using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace LevelManager
{
    public static class LevelLoader
    {
        public static GameInfo giGameInfo;
        private static string sFileName = "LevelInfo.json";

        private static bool bSetupComplete = false;
        private static int iLevelChosen = -1;
        public static void SetupLevelManager()
        {
            if (bSetupComplete)
            {
                return;
            }
            string DirectoryPath = Application.dataPath.Replace("/", "\\\\");
            DirectoryPath = Path.Combine(DirectoryPath, "level_info");
            string LevelInfoPath = Path.Combine(DirectoryPath, sFileName);
            bSetupComplete = File.Exists(LevelInfoPath);
            if (!bSetupComplete)
            {
                if (Directory.CreateDirectory(DirectoryPath) == null)
                {
                    Debug.LogError($"{DirectoryPath} directory failed at creation");
                }
                using (FileStream _DummyStream = File.Create(LevelInfoPath, 4096, FileOptions.WriteThrough))
                {
                    if (_DummyStream == null)
                    {
                        Debug.LogError($"{LevelInfoPath} file failed at creation");
                    }
                    else
                    {
                        Debug.Log($"File {LevelInfoPath} created successfully!");
                        bSetupComplete = true;
                        _DummyStream.Close();
                    }
                }
            }
            LoadLevels();
            SceneManager.sceneLoaded += PassInfo;
        }

        [Serializable]
        private struct LevelFileNames
        {
            public string[] Levels;
        }
        public static void LoadLevels()
        {
            string DirectoryPath = Path.Combine(Application.dataPath, "level_info");
            string LevelInfoPath = Path.Combine(DirectoryPath, sFileName);
            string LevelInfoString = File.ReadAllText(LevelInfoPath);

            try
            {
                giGameInfo = new GameInfo();
                LevelFileNames LevelNames = JsonUtility.FromJson<LevelFileNames>(LevelInfoString);
                foreach (string LName in LevelNames.Levels)
                {
                    giGameInfo.ParseLevel(LName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during deserialization: {ex.Message}");
            }
        }

        public static void LoadLevel(int iIdx)
        {
            iLevelChosen = iIdx;
            SceneManager.LoadScene(1);
        }

        public static void PassInfo(Scene sScene, LoadSceneMode _lsmMode)
        {
            if (sScene.name != "GameScene")
            {
                return;
            }

            GameObject[] CurGameManagers = GameObject.FindGameObjectsWithTag("GameManager");
            if (CurGameManagers.Length < 1)
            {
                return;
            }

            IGameManager GM = CurGameManagers[0].GetComponent<IGameManager>();
            if (GM == null)
            {
                return;
            }
            if (iLevelChosen != 1000)
            {
                LevelInfo CurLevelInfo = GetMusicInfo(iLevelChosen);
                LevelEditor.SetCurFileLevel(giGameInfo.GetLevelName(iLevelChosen), false, CurLevelInfo, GM.GetAudioSource());
                GM.SetCurrentLevel(CurLevelInfo);
            }
        }

        public static LevelInfo GetMusicInfo(int iIdx)
        {
            return giGameInfo.Levels[iIdx];
        }
    }
}

