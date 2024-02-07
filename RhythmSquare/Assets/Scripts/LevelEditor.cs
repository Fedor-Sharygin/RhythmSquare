#if UNITY_STANDALONE || UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LevelManager
{
    public static class LevelEditor
    {
        private static RecordAnimation goRecordSymbol;
        public static void SetRecordingSymbol(RecordAnimation goNRecordSymbol)
        {
            goRecordSymbol = goNRecordSymbol;
        }

        public static bool bLevelEditingState { private set; get; } = false;
        public static void FlipEditState()
        {
            if (goRecordSymbol)
            {
                goRecordSymbol.FlipRecording();
            }
            bLevelEditingState = !bLevelEditingState;

            if (!bLevelEditingState)
            {
                SaveToFile();
            }
        }

        private static string sCurLevelFile;
        private static LevelManager.LevelInfo liLevel;
        private static AudioSource auSource;
        private static bool bIntroRecording;
        private static List<float> lfTimeStamps = new List<float>();
        public static void SetCurFileLevel(string sFileLevel, bool bRecordState, LevelManager.LevelInfo liNLevel, AudioSource auNSource)
        {
            sCurLevelFile = sFileLevel;
            bIntroRecording = bRecordState;
            liLevel = liNLevel;
            auSource = auNSource;
            lfTimeStamps.Clear();
        }

        public static void AddBeat()
        {
            lfTimeStamps.Add(auSource.time);
        }

        private static void SaveToFile()
        {
            List<LevelManager.TickInfo> NTickList = new List<LevelManager.TickInfo>();
            foreach (float TimeStamp in lfTimeStamps)
            {
                NTickList.Add(new LevelManager.TickInfo(TimeStamp));
            }

            if (bIntroRecording)
            {
                liLevel.IntroBeats = NTickList.ToArray();
            }
            else
            {
                liLevel.GameBeats = NTickList.ToArray();
            }

            string DirectoryPath = Path.Combine(Application.streamingAssetsPath, "level_info");
            string FilePath = Path.Combine(DirectoryPath, sCurLevelFile + ".json");
            string JsonDesc = JsonUtility.ToJson(liLevel, true);
            File.WriteAllText(FilePath, JsonDesc);
        }
    }
}

#endif