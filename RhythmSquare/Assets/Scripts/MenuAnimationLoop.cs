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
    private int iClipIdx;
    public void StartAudio(int iAudioIdx)
    {
        iClipIdx = iAudioIdx;
        StartCoroutine("ChangeAudio");
    }

    public IEnumerator ChangeAudio()
    {
        CurLevelInfo = LevelManager.LevelLoader.GetMusicInfo(iClipIdx);

        #if UNITY_STANDALONE  || UNITY_EDITOR
        LevelManager.LevelEditor.SetCurFileLevel(LevelManager.LevelLoader.giGameInfo.GetLevelName(iClipIdx),
            true, CurLevelInfo, auSource);
        #endif

        yield return GlobalNamespace.GlobalMethods.GetAudioClip(CurLevelInfo.MusicName,
            acAudioClip =>
            {
                auSource.clip = acAudioClip;
                auSource.loop = true;
                iIntroBeatIdx = 0;
                bReceivedAudio = true;
                auSource.Play();
            }
        );
    }

    [SerializeField]
    private Animator aSquareAnim;
    [SerializeField]
    private SpriteRenderer srSquareSprite;
    private int iIntroBeatIdx;
    private bool bReceivedAudio = false;
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

        if (!bReceivedAudio)
        {
            return;
        }

        if (Mathf.Abs(auSource.time - CurLevelInfo.IntroBeats[iIntroBeatIdx].TimeStamp) >= .05f)
        {
            return;
        }

        //srSquareSprite.color = CurLevelInfo.IntroBeats[iIntroBeatIdx].SquareColor;
        srSquareSprite.color = new Color(Random.Range(.2f, .9f),
                                         Random.Range(.2f, .9f),
                                         Random.Range(.2f, .9f));
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
