using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IGameManager
{
    public void GetPoints(int iPoints, Color cSquareColor);
    public void SetCurrentLevel(int iLevelIdx);
    public void ParseInformation();
    public IEnumerator PassAudio();
    public AudioSource GetAudioSource();
}
