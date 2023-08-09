using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class OnEnabledGameObject : iTweenEditor
{

    // public SkeletonGraphic animxGraphic;
    // public SkeletonAnimation animxAnimation;

    // void Awake()
    // {
    //     animx = gameObject.GetComponent<SkeletonGraphic>();
    //    // animxAnimation = gameObject.GetComponent<SkeletonAnimation>();
    // }

    void Start()
    {
    }

    [System.Serializable]
    public class OnStart : UnityEvent
    {

    };
    [System.Serializable]
    public class OnComplete : UnityEvent
    {

    };

    public OnStart onStart;
    public OnComplete onComplete;

    void OnEnable()
    {
        if (onStart != null) onStart.Invoke();
    }

    // public void setAnimationGraphic(string nameAnimation)
    // {
    //     animxGraphic = gameObject.GetComponent<SkeletonGraphic>();
    //     if (animxGraphic != null && animxGraphic.AnimationState != null)
    //     {
    //         animxGraphic.AnimationState.SetAnimation(0, nameAnimation, false);
    //         float f = animxGraphic.SkeletonDataAsset.GetSkeletonData(true).FindAnimation(nameAnimation).Duration;
    //         StartCoroutine(onFinish(f + 0.1f));
    //     }
    // }

    // public void setAnimation(string nameAnimation)
    // {
    //     animxAnimation = gameObject.GetComponent<SkeletonAnimation>();
    //     if (animxAnimation != null && animxAnimation.state != null)
    //     {
    //         animxAnimation.state.SetAnimation(0, nameAnimation, false);
    //         float f = animxAnimation.SkeletonDataAsset.GetSkeletonData(true).FindAnimation(nameAnimation).Duration;
    //         StartCoroutine(onFinish(f + 0.1f));
    //     }
    // }

    IEnumerator onFinish(float time)
    {
        yield return new WaitForSeconds(time);
        onComplete.Invoke();
    }

    public void finish(float time)
    {
        StartCoroutine(onFinish(time));
    }
}
