using LevelManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{

    #if false
    [SerializeField]
    private float fIntroTime;
    [SerializeField]
    private AudioClip aCurLevelMusic;
    [SerializeField]
    private int iBPM;
    [SerializeField]
    private int iBarSignatureCnt;
    private float fBeatTime;
    private float fBarTime;
    #endif

    [SerializeField]
    private GameObject pfRhythmTick;
    [SerializeField]
    private AudioSource auSource;
    [SerializeField]
    private TMPro.TextMeshProUGUI tPointsText;
    private int iCurPoints = 0;
    [SerializeField]
    private TMPro.TextMeshProUGUI tMaxPointsText;
    [SerializeField]
    private AudioSource auGood;
    [SerializeField]
    private AudioSource auBad;

    private Animator aSquareAnimator;
    private SpriteRenderer srSqureSprite;
    private LevelInfo mlLevelInfo;
    private void Start()
    {
        aSquareAnimator = GetComponentInChildren<Animator>();
        srSqureSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        RhythmTick.GrantPointsEvent += GetPoints;
        ParseInformation();
        //StartCoroutine("ParseInformation");
        if (tPointsText != null)
        {
            tPointsText.text = iCurPoints.ToString();
        }
    }

    private void OnDestroy()
    {
        RhythmTick.GrantPointsEvent -= GetPoints;
    }

    public void GetPoints(int iPoints)
    {
        iCurPoints += iPoints;
        if (tPointsText != null)
        {
            tPointsText.text = iCurPoints.ToString();
        }
        if (iPoints > 0 && auGood != null)
        {
            auGood.Stop();
            auGood.Play();
            auGood.time = .106f;
        }
        if (iPoints < 0 && auBad != null)
        {
            auBad.Stop();
            auBad.Play();
        }

        aSquareAnimator.SetTrigger(iPoints < 0 ? "Bad" : "Bounce");
        //srSqureSprite.color = new Color(Random.Range(.2f, .9f),
        //                                Random.Range(.2f, .9f),
        //                                Random.Range(.2f, .9f));
        srSqureSprite.color = iPoints < 0 ? Color.grey : GlobalNamespace.GlobalMethods.cCurFrameColor;
    }

    private int iCurLevelIdx;
    public void SetCurrentLevel(int iLevelIdx)
    {
        iCurLevelIdx = iLevelIdx;
        mlLevelInfo = LevelLoader.giGameInfo.Levels[iLevelIdx];
        StartCoroutine("PassAudio");
    }

    private List<RhythmTick> lTickList = new List<RhythmTick>();
    public void ParseInformation()
    {
    #if false
        fBeatTime = 60f / (float)iBPM;
        fBarTime = fBeatTime * (float)iBarSignatureCnt;

        //basic beat version
        float StartBeatPos = fIntroTime;
        float EndBeatPos = aCurLevelMusic.length;
        int BeatCnt = (int)((EndBeatPos - StartBeatPos) / fBeatTime);
        for (float CurBeatPos = StartBeatPos; CurBeatPos <= EndBeatPos; CurBeatPos += fBeatTime)
        {
            GameObject CurTick = Instantiate(pfRhythmTick,
                new Vector3(CurBeatPos, -3.2f),
                Quaternion.identity,
                transform.GetChild(1));
            CurTick.GetComponent<RhythmTick>().fSpeed = -1f;
            GameObject OppTick = Instantiate(pfRhythmTick,
                new Vector3(-CurBeatPos, -3.2f),
                Quaternion.identity,
                transform.GetChild(1));
        }
    #endif
    #if true
        float TickSpeed = pfRhythmTick.GetComponent<RhythmTick>().fSpeed;
        foreach (TickInfo CurTickInfo in mlLevelInfo.GameBeats)
        {
            GameObject CurTick = Instantiate(pfRhythmTick,
                new Vector3(CurTickInfo.TimeStamp * TickSpeed, -3.2f),
                Quaternion.identity,
                transform.GetChild(1));
            RhythmTick CTInfo = CurTick.GetComponent<RhythmTick>();
            lTickList.Add(CTInfo);
            CTInfo.fSpeed = -TickSpeed;
            CTInfo.cSquareColor = CurTickInfo.SquareColor;

            //Opposite Tick
            GameObject OppTick = Instantiate(pfRhythmTick,
                new Vector3(-CurTickInfo.TimeStamp * TickSpeed, -3.2f),
                Quaternion.identity,
                transform.GetChild(1));
            RhythmTick OTInfo = OppTick.GetComponent<RhythmTick>();
            lTickList.Add(OTInfo);
            OTInfo.cSquareColor = CurTickInfo.SquareColor;
        }
        if (tMaxPointsText != null)
        {
            tMaxPointsText.text = LevelLoader.giGameInfo.MaxPoints[iCurLevelIdx].ToString();
        }
    #endif
    }

    public IEnumerator PassAudio()
    {
        yield return GlobalNamespace.GlobalMethods.GetAudioClip(mlLevelInfo.MusicName,
            acAudioClip =>
            {
                auSource.clip = acAudioClip;
                auSource.loop = false;
                auSource.Play();

                foreach (RhythmTick rTick in lTickList)
                {
                    rTick.bMove = true;
                }
            }
        );
    }

    public AudioSource GetAudioSource()
    {
        return auSource;
    }

    [SerializeField]
    private float fExitDelay = 2f;
    private void Update()
    {
        if (auSource.isPlaying)
        {
            return;
        }

        int CurPoints = int.Parse(tMaxPointsText.text);
        if (tMaxPointsText != null && CurPoints < iCurPoints)
        {
            CurPoints += 5;
            tMaxPointsText.text = Mathf.Min(CurPoints, iCurPoints).ToString();
            return;
        }

        fExitDelay -= Time.deltaTime;
        if (fExitDelay > 0f)
        {
            return;
        }

        LevelLoader.giGameInfo.SetMaxPointsForLevel(iCurLevelIdx, int.Parse(tMaxPointsText.text));
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
