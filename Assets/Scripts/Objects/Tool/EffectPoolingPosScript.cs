using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolingPosScript : MonoBehaviour
{
    public static EffectPoolingPosScript instance;

    public PoolingScript smokePoolingScript;
    public PoolingScript heartEmojiPoolingScript, happyEmojiPoolingScript, sadEmojiPoolingScript;
    [Space(15)]
    public PoolingScript moneySpentEffectPoolingScript;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    { }
}
