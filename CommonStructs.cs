using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace LevelManager
{
    [Serializable]
    public struct TickInfo
    {
        public float TimeStamp;
        public Color SquareColor;

        public TickInfo(float NTimeStamp)
        {
            TimeStamp = NTimeStamp;
            SquareColor = Color.red;
        }
    }
    [Serializable]
    public struct LevelInfo
    {
        public string MusicName;
        public string LevelName;
        public TickInfo[] GameBeats;
        public TickInfo[] IntroBeats;
    }
    public class GameInfo
    {
        public GameInfo()
        {
            saLevelNames = new List<string>();
            Levels = new List<LevelInfo>();
            MaxPoints = new List<int>();
        }

        public void ParseLevel(string sLevelFileName, int iMaxPoints)
        {
            if (saLevelNames.Contains(sLevelFileName))
            {
                return;
            }

            Debug.Log($"LOG: Creation [{sLevelFileName}] file - start");

            saLevelNames.Add(sLevelFileName);

            string DirectoryPath = Path.Combine(Application.streamingAssetsPath, "level_info");
            string LevelInfoPath = Path.Combine(DirectoryPath, sLevelFileName + ".json");
            string LevelInfoString;
            
            #if UNITY_STANDALONE || UNITY_EDITOR
            
            LevelInfoString = File.ReadAllText(LevelInfoPath);
            
            #else
            
            UnityWebRequest www = UnityWebRequest.Get(LevelInfoPath);
            www.SendWebRequest();
            while (!www.isDone);
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"LOG: File found at [{LevelInfoPath}]");
                LevelInfoString = www.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning($"LOG: Error while parsing [{LevelInfoPath}] file: [{www.error}]");
                LevelInfoString = File.ReadAllText(LevelInfoPath);
            }
            //LevelInfoString = www.downloadHandler.text;
            
            #endif

            Debug.Log($"LOG: Current Level info is: [{LevelInfoString}]");

            LevelInfo NLevelInfo = JsonUtility.FromJson<LevelInfo>(LevelInfoString);
            Levels.Add(NLevelInfo);

            MaxPoints.Add(iMaxPoints);
            Debug.Log($"LOG: Creation [{sLevelFileName}] file - end");
        }

        #if UNITY_WEBGL

        public void ParseLevel(string sLevelFileName, string sLevelDescription)
        {
            if (saLevelNames.Contains(sLevelFileName))
            {
                return;
            }

            saLevelNames.Add(sLevelFileName);
            
            LevelInfo NLevelInfo = JsonUtility.FromJson<LevelInfo>(sLevelDescription);
            Levels.Add(NLevelInfo);

            MaxPoints.Add(0);
        }

        #endif

        public string GetLevelName(int iCurIdx)
        {
            if (iCurIdx < 0 || iCurIdx >= saLevelNames.Count)
            {
                return "";
            }
            return saLevelNames[iCurIdx];
        }

        public void SetMaxPointsForLevel(int iLvlIdx, int iMaxPts)
        {
            MaxPoints[iLvlIdx] = iMaxPts;
        }

        public List<LevelInfo> Levels { private set; get; }
        public List<string> saLevelNames { private set; get; }
        public List<int> MaxPoints;
    }
}
