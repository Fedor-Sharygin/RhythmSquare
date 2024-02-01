using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IGameManager
{
    public void GetPoints(int iPoints, Color cSquareColor);
    public void SetCurrentLevel(LevelManager.LevelInfo liInformation);
    public void ParseInformation();
    public IEnumerator PassAudio();
    public AudioSource GetAudioSource();
}
