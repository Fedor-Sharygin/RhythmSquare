using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace GlobalNamespace
{
    public static class GlobalMethods
    {
        public static IEnumerator GetAudioClip(string sMusicName, Action<AudioClip> aCallback)
        {
            string FilePath = Path.Combine(ReadOnlyFolder, sMusicName);
            
            #if UNITY_STANDALONE || UNITY_EDITOR
            
            FilePath = $"file://{FilePath}";
            
            #endif
            
            UnityWebRequest UWR = UnityWebRequestMultimedia.GetAudioClip(FilePath, AudioType.WAV);
            yield return UWR.SendWebRequest();

            if (UWR.result == UnityWebRequest.Result.ConnectionError
             || UWR.result == UnityWebRequest.Result.ProtocolError
             || UWR.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"Error loading audio: {UWR.error}");
            }
            else
            {
                AudioClip CurAC = DownloadHandlerAudioClip.GetContent(UWR);
                aCallback?.Invoke(CurAC);
            }
        }

        public static Color cCurFrameColor { get; private set; }
        public static void SetNewColor()
        {
            cCurFrameColor = new Color(UnityEngine.Random.Range(.2f, .9f),
                                       UnityEngine.Random.Range(.2f, .9f),
                                       UnityEngine.Random.Range(.2f, .9f));
        }


        public static string ReadOnlyFolder { private set; get; } = Path.Combine(Application.streamingAssetsPath, "level_info");
        public static string PersistentFolder { private set; get; } = Path.Combine(Application.persistentDataPath, "level_info");
    }
}
