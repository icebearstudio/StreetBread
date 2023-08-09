using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimScript : AnimScript
{

    new void Awake()
    {
        base.Awake();
    }

    new void Start()
    {
        base.Start();
    }

    public void setAnim(string nameAnimation, bool loop = false, float delay = 0f, float crossFadeTime = 0, bool isNextAnim = false, string nextAnimation = "", bool loopNextAnimation = false, float crossFadeTimeNextAnim = 0)
    {
        setAnim(mainAnim, true, nameAnimation, loop, delay, crossFadeTime, isNextAnim, nextAnimation, loopNextAnimation, crossFadeTimeNextAnim);
    }
}
