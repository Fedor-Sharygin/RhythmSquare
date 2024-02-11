#if UNITY_STANDALONE || UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAnimation : MonoBehaviour
{
    private SpriteRenderer srSprite;

    private void Awake()
    {
        srSprite = GetComponent<SpriteRenderer>();
        LevelManager.LevelEditor.SetRecordingSymbol(this);
        FlipRecording();
    }

    private bool bRecording = true;
    public void FlipRecording()
    {
        bRecording = !bRecording;
        if (!bRecording)
        {
            Color NColor = srSprite.color;
            NColor.a = 0f;
            srSprite.color = NColor;
        }
    }

    private float fAlphaTime = 0f;
    private void Update()
    {
        if (!bRecording)
        {
            return;
        }

        fAlphaTime += Time.deltaTime;
        Color NColor = srSprite.color;
        NColor.a = .75f + .2f * Mathf.Sin(fAlphaTime);
        srSprite.color = NColor;
    }
}

#endif