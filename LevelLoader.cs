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
        private static string LevelInfoPath = Path.Combine(GlobalNamespace.GlobalMethods.PersistentFolder, "LevelInfo.json");
        private static string StrLvlInfoPath = Path.Combine(GlobalNamespace.GlobalMethods.ReadOnlyFolder, "LevelInfo.json");

        private static void CreatePDPFile(string LevelInfo)
        {
            Debug.Log($"Creating file [{LevelInfoPath}]");
            if (Directory.CreateDirectory(GlobalNamespace.GlobalMethods.PersistentFolder) == null)
            {
                Debug.LogError($"{GlobalNamespace.GlobalMethods.PersistentFolder} directory failed at creation");
            }
            FileStream _DummyStream = File.Create(LevelInfoPath, 4096, FileOptions.WriteThrough | FileOptions.Asynchronous);
            if (_DummyStream == null)
            {
                Debug.LogError($"File [{LevelInfoPath}] creation fail!");
            }
            else
            {
                Debug.Log($"File [{LevelInfoPath}] creation success!");
                _DummyStream.Close();
            }
            File.WriteAllText(LevelInfoPath, LevelInfo);
        }

        private static bool bCurInSetupMode = false;
        private static bool bSetupComplete = false;
        private static int iLevelChosen = -1;
        public static void SetupLevelManager()
        {
            if (bCurInSetupMode || bSetupComplete)
            {
                return;
            }
            bCurInSetupMode = true;
            LoadLevels();
            SceneManager.sceneLoaded += PassInfo;
            Application.quitting += SaveMaxPoints;
            bCurInSetupMode = false;
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
            string sCurLevelPath = LevelInfoPath;
            bool bNoFileAvailable;
            if (bNoFileAvailable = !File.Exists(sCurLevelPath))
            {
                Debug.Log($"LOG: File not found at [{LevelInfoPath}] - using [{StrLvlInfoPath}]");
                sCurLevelPath = StrLvlInfoPath;
            }
            
            #if UNITY_ANDROID || UNITY_IOS

            #if UNITY_IOS
            sCurLevelPath = "file://" + sCurLevelPath;
            #endif

            UnityWebRequest www = UnityWebRequest.Get(sCurLevelPath);
            www.SendWebRequest();
            while (!www.isDone);
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"LOG: File found at [{sCurLevelPath}]");
                LevelInfoString = www.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning($"LOG: Error while parsing [{sCurLevelPath}] file: [{www.error}]");
                LevelInfoString = File.ReadAllText(sCurLevelPath);
            }
            
            #elif UNITY_STANDALONE || UNITY_EDITOR
            
            LevelInfoString = File.ReadAllText(sCurLevelPath);
            
            #endif

            Debug.Log($"LOG: Current info string: [{LevelInfoString}]");
            
            if (bNoFileAvailable)
            {
                CreatePDPFile(LevelInfoString);
            }

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
                Debug.LogError($"LOG: Error during deserialization: {ex.Message}");
            }

            bSetupComplete = true;
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
                #if !UNITY_WEBGL
                if (bSetupComplete)
                {
                    SaveMaxPoints();
                }
                #endif
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
            #if !UNITY_WEBGL
            LevelFileNames LevelNames = new LevelFileNames();
            LevelNames.Levels = giGameInfo.saLevelNames.ToArray();
            LevelNames.MaxPoints = giGameInfo.MaxPoints.ToArray();

            string JsonDesc = JsonUtility.ToJson(LevelNames, true);
            File.WriteAllText(LevelInfoPath, JsonDesc);
            #endif
        }
    }
}

