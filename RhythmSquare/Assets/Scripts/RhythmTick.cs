using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RhythmTick : MonoBehaviour
{
    public static event Action<int, Color> GrantPointsEvent = delegate { };

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
