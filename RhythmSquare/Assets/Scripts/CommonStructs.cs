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

            saLevelNames.Add(sLevelFileName);

            string DirectoryPath = Path.Combine(Application.dataPath, "level_info");
            string LevelInfoPath = Path.Combine(DirectoryPath, sLevelFileName + ".json");
            string LevelInfoString = File.ReadAllText(LevelInfoPath);
            LevelInfo NLevelInfo = JsonUtility.FromJson<LevelInfo>(LevelInfoString);
            Levels.Add(NLevelInfo);

            MaxPoints.Add(iMaxPoints);
        }

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
