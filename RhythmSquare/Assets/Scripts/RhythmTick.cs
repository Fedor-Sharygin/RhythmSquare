using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RhythmTick : MonoBehaviour
{
    public static event Action<int> GrantPointsEvent = delegate { };

    private Transform tSpriteTransform;
    private Vector3 BaseScale = new Vector3(.5f, .5f);
    private void Awake()
    {
        tSpriteTransform = transform.GetChild(0);
        if (tSpriteTransform != null)
        {
            tSpriteTransform.localScale = BaseScale;
        }
    }

    private bool bDeduct = false;
    public Color cSquareColor { set; private get; } = Color.white;
    private void Update()
    {
        if (transform.position.x * Mathf.Sign(fSpeed) > .1f)
        {
            bDeduct = true;
            Destroy(gameObject);
            return;
        }

        Move();
    }

    public bool bMove = false;
    public float fSpeed = 1;
    private void Move()
    {
        if (!bMove)
        {
            return;
        }
        transform.position += new Vector3(fSpeed * Time.deltaTime, 0f);

        if (!tSpriteTransform)
        {
            return;
        }
        float DistToCenter = (Mathf.Abs(transform.position.x) + 1) / 3f * Mathf.PI;
        float ScaleEffect = Mathf.Sin(DistToCenter) + 1;
        tSpriteTransform.localScale = BaseScale + 2f * ScaleEffect * Vector3.one;
    }

    private void OnDestroy()
    {
        int Points = bDeduct ? -5 : GlobalNamespace.GlobalMethods.GetGamePointPoints(gameObject);
        GrantPointsEvent(Points);
        if (bDeduct)
        {
            GameObject.FindGameObjectWithTag("RhythmTarget")?.GetComponent<RhythmTarget>()?.RemoveTickFail();
        }
    }
}
