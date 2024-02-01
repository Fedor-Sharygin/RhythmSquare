using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmTarget : MonoBehaviour
{
    private List<GameObject> lgoRhythmTicks;

    private void Awake()
    {
        lgoRhythmTicks = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D cCollision)
    {
        lgoRhythmTicks.Add(cCollision.gameObject);
    }


#if UNITY_ANDROID || UNITY_IOS
    private int PrevTouchCount = 0;
#endif
    private void Update()
    {
        #if UNITY_ANDROID || UNITY_IOS
        
        if (Input.touchCount - PrevTouchCount > 1)
        {
            StartCoroutine("PressTick");
        }
        PrevTouchCount = Input.touchCount;

        #else
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LevelManager.LevelEditor.FlipEditState();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!LevelManager.LevelEditor.bLevelEditingState)
            {
                StartCoroutine("PressTick");
            }
            else
            {
                LevelManager.LevelEditor.AddBeat();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        #endif
    }

    private void PressTick()
    {
        if (lgoRhythmTicks.Count <= 1)
        {
            return;
        }

        lgoRhythmTicks[0].GetComponent<RhythmTick>()?.DisableCollision();
        lgoRhythmTicks[1].GetComponent<RhythmTick>()?.DisableCollision();

        Destroy(lgoRhythmTicks[0]);
        Destroy(lgoRhythmTicks[1]);
        lgoRhythmTicks.RemoveRange(0, 2);
    }

    public void RemoveTickFail()
    {
        if (lgoRhythmTicks.Count < 1)
        {
            return;
        }
        lgoRhythmTicks.RemoveAt(0);
    }
}
