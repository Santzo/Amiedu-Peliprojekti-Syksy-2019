using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite
{
    public EnemySprite(Transform trans, SpriteRenderer sr, Color oriColor)
    {
        this.trans = trans;
        this.sr = sr;
        this.oriColor = oriColor;
    }

    public Transform trans;
    public SpriteRenderer sr;
    public Color oriColor;
}
