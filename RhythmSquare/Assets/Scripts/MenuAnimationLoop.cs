using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationLoop : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] auMainThemes;
    [SerializeField]
    private AudioSource auSource;
    private void Awake()
    {
        LevelManager.LevelLoader.SetupLevelManager();
        if ( LevelManager.LevelLoader.giGameInfo.Levels == null
          || LevelManager.LevelLoader.giGameInfo.Levels.Count == 0)
        {
            return;
        }

        StartCoroutine("StartAudio");


        GameObject RecCirc = GameObject.FindGameObjectWithTag("RecordSymbol");
        if (RecCirc != null)
        {
            #if UNITY_STANDALONE || UNITY_EDITOR
            DontDestroyOnLoad(RecCirc);
            #else
            Destroy(RecCirc);
            #endif
        }
    }

    private LevelManager.LevelInfo CurLevelInfo;
    private IEnumerator StartAudio()
    {
        int AudioClipIdx = Random.Range(0, LevelManager.LevelLoader.giGameInfo.Levels.Count);
        CurLevelInfo = LevelManager.LevelLoader.GetMusicInfo(AudioClipIdx);

        #if UNITY_STANDALONE  || UNITY_EDITOR
        LevelManager.LevelEditor.SetCurFileLevel(LevelManager.LevelLoader.giGameInfo.GetLevelName(AudioClipIdx),
            true, CurLevelInfo, auSource);
        #endif

        yield return GlobalNamespace.GlobalMethods.GetAudioClip(CurLevelInfo.MusicName,
            acAudioClip =>
            {
                auSource.clip = acAudioClip;
                auSource.loop = true;
                auSource.Play();
            }
        );
    }

    [SerializeField]
    private Animator aSquareAnim;
    [SerializeField]
    private SpriteRenderer srSquareSprite;
    private int iIntroBeatIdx;
    private void Update()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LevelManager.LevelEditor.FlipEditState();
        }

        if (!LevelManager.LevelEditor.bLevelEditingState)
        {
        #endif


        if (Mathf.Abs(auSource.time - CurLevelInfo.IntroBeats[iIntroBeatIdx].TimeStamp) >= .05f)
        {
            return;
        }

        srSquareSprite.color = CurLevelInfo.IntroBeats[iIntroBeatIdx].SquareColor;
        aSquareAnim.SetTrigger("Bounce");
        iIntroBeatIdx = (iIntroBeatIdx + 1) % CurLevelInfo.IntroBeats.Length;


        #if UNITY_STANDALONE || UNITY_EDITOR
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LevelManager.LevelEditor.AddBeat();
            }

            if (auSource.clip.length - auSource.time < .05f)
            {
                LevelManager.LevelEditor.FlipEditState();
            }
        }
        #endif
    }


}
