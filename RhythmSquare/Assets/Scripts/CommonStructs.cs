using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
        public TickInfo[] GameBeats;
        public TickInfo[] IntroBeats;
    }
    [Serializable]
    public class GameInfo
    {
        public GameInfo()
        {
            saLevelNames = new List<string>();
            Levels = new List<LevelInfo>();
        }

        public void ParseLevel(string sLevelFileName)
        {
            if (saLevelNames.Contains(sLevelFileName))
            {
                return;
            }

            saLevelNames.Add(sLevelFileName);

            string DirectoryPath = Path.Combine(Application.dataPath, "level_info");
            string LevelInfoPath = Path.Combine(DirectoryPath, sLevelFileName + ".json");
            string LevelInfoString = File.ReadAllText(LevelInfoPath);
            Levels.Add(JsonUtility.FromJson<LevelInfo>(LevelInfoString));
        }

        public string GetLevelName(int iCurIdx)
        {
            if (iCurIdx < 0 || iCurIdx >= saLevelNames.Count)
            {
                return "";
            }
            return saLevelNames[iCurIdx];
        }

        public List<LevelInfo> Levels { private set; get; }
        private List<string> saLevelNames;
    }
}
