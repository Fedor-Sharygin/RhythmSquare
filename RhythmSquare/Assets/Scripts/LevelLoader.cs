using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.Networking;

namespace LevelManager
{
    public static class LevelLoader
    {
        public static GameInfo giGameInfo;
        private static string DirectoryPath = DirectoryPath = Path.Combine(Application.streamingAssetsPath, "level_info");
        private static string LevelInfoPath = Path.Combine(DirectoryPath, "LevelInfo.json");

        private static bool bSetupComplete = false;
        private static int iLevelChosen = -1;
        public static void SetupLevelManager()
        {
            if (bSetupComplete)
            {
                return;
            }

            #if UNITY_STANDALONE || UNITY_EDITOR
            
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
            
            #else
            
            bSetupComplete = true;
            
            #endif

            LoadLevels();
            SceneManager.sceneLoaded += PassInfo;
            Application.quitting += SaveMaxPoints;
        }

        [Serializable]
        private struct LevelFileNames
        {
            public string[] Levels;
            public int[] MaxPoints;
        }
        public static void LoadLevels()
        {
            string LevelInfoString;
            
            #if UNITY_ANDROID

            //Debug.Log(LevelInfoPath);
            UnityWebRequest www = UnityWebRequest.Get(LevelInfoPath);
            www.SendWebRequest();
            while (!www.isDone);
            LevelInfoString = www.downloadHandler.text;
            
            #elif UNITY_STANDALONE || UNITY_EDITOR
            
            LevelInfoString = File.ReadAllText(LevelInfoPath);
            
            #endif

            try
            {
                giGameInfo = new GameInfo();
                LevelFileNames LevelNames = JsonUtility.FromJson<LevelFileNames>(LevelInfoString);
                int LvlIdx = 0;
                foreach (string LName in LevelNames.Levels)
                {
                    giGameInfo.ParseLevel(LName, LevelNames.MaxPoints[LvlIdx++]);
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
            SceneManager.LoadScene("GameScene");
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
                #if UNITY_STANDALONE || UNITY_EDITOR
                LevelEditor.SetCurFileLevel(giGameInfo.GetLevelName(iLevelChosen),
                    false, GetMusicInfo(iLevelChosen), GM.GetAudioSource());
                #endif
                GM.SetCurrentLevel(iLevelChosen);
            }
        }

        public static LevelInfo GetMusicInfo(int iIdx)
        {
            return giGameInfo.Levels[iIdx];
        }

        private static void SaveMaxPoints()
        {
            LevelFileNames LevelNames = new LevelFileNames();
            LevelNames.Levels = giGameInfo.saLevelNames.ToArray();
            LevelNames.MaxPoints = giGameInfo.MaxPoints.ToArray();

            string JsonDesc = JsonUtility.ToJson(LevelNames, true);
            File.WriteAllText(LevelInfoPath, JsonDesc);
        }
    }
}

