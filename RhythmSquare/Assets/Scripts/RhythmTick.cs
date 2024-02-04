using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RhythmTick : MonoBehaviour
{
    public static event Action<int, Color> GrantPointsEvent = delegate { };

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
    private bool bCurrDestroyed = false;
    private void OnCollisionExit2D(Collision2D cOther)
    {
        if (bCurrDestroyed)
        {
            return;
        }
        if (cOther.gameObject.tag == "RhythmTick")
        {
            bDeduct = true;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
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
        tSpriteTransform.localScale = BaseScale + 1.4f * ScaleEffect * Vector3.one;
    }

    private void OnDestroy()
    {
        int Points = bDeduct ? -5 : 10 - 2 * Mathf.FloorToInt(Mathf.Abs(transform.position.x) / .05f);
        Color SquareColor = bDeduct ? Color.grey : cSquareColor;
        GrantPointsEvent(Points, SquareColor);
        if (bDeduct)
        {
            GameObject.FindGameObjectWithTag("RhythmTarget")?.GetComponent<RhythmTarget>()?.RemoveTickFail();
        }
    }

    public void DisableCollision()
    {
        bCurrDestroyed = true;
    }
}
