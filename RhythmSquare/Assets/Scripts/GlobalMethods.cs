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
            string FilePath = Path.Combine(Application.dataPath, "level_info", sMusicName);
            UnityWebRequest UWR = UnityWebRequestMultimedia.GetAudioClip($"file://{FilePath}", AudioType.WAV);
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

        [Obsolete]
        public static IEnumerator<AudioClip> GetAudioClip(string sMusicName)
        {
            string FilePath = Path.Combine(Application.dataPath, "level_info", sMusicName);
            WWW w = new WWW($"file://{FilePath}");
            yield return w.GetAudioClip(false, false);
        }
    }
}
