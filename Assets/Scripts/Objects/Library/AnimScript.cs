using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimScript : ToolMoveScript
{
    public Animator mainAnim;
    public List<(string, float, List<(string, float, string)>, AnimationClip)> animationClipDurationList = new List<(string, float, List<(string, float, string)>, AnimationClip)>();  // name anim - duration - list eventCall (event name-time-string value) - AnimationClip
    public string animCurrentStatus;
    public int frameProcessCurrent;

    public void Awake()
    {
        if (mainAnim == null) mainAnim = GetComponent<Animator>();
        if (mainAnim == null && transform.childCount > 0) mainAnim = transform.GetChild(0).GetComponent<Animator>();
        //if (mainAnim == null) mainAnim = GetComponentInChildren<Animator>();
    }

    public void Start()
    {
        if (mainAnim != null)
            loadAnimClipDuration();
    }

    public void loadAnimClipDuration()
    {
        AnimationClip[] clips = mainAnim.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            AnimationEvent[] animationEvents = clips[i].events;
            List<(string, float, string)> eventList = new List<(string, float, string)>();
            foreach (AnimationEvent e in animationEvents)
            {
                eventList.Add((e.functionName, e.time, e.stringParameter));
            }
            animationClipDurationList.Add((clips[i].name, clips[i].length, eventList, clips[i]));
        }
    }

    public void setAnim(Animator animator, bool isMainAnimator, string nameAnimation, bool loop = false, float delay = 0f, float crossFadeTime = 0, bool isNextAnim = false, string nextAnimation = "", bool loopNextAnimation = false, float crossFadeTimeNextAnim = 0)
    {
        if (delaySetAnimCoroutine != null && isMainAnimator)
        {
            this.StopCoroutine(delaySetAnimCoroutine);
            delaySetAnimCoroutine = null;
        }
        if (isMainAnimator)
            delaySetAnimCoroutine = StartCoroutine(delaySetAnim(animator, isMainAnimator, nameAnimation, loop, delay, crossFadeTime, isNextAnim, nextAnimation, loopNextAnimation, crossFadeTimeNextAnim));
        else
            StartCoroutine(delaySetAnim(animator, isMainAnimator, nameAnimation, loop, delay, crossFadeTime, isNextAnim, nextAnimation, loopNextAnimation, crossFadeTimeNextAnim));
    }

    Coroutine delaySetAnimCoroutine = null;
    IEnumerator delaySetAnim(Animator animator, bool isMainAnimator, string nameAnimation, bool loop = false, float delay = 0f, float crossFadeTime = 0, bool isNextAnim = false, string nextAnimation = "", bool loopNextAnimation = false, float crossFadeTimeNextAnim = 0)
    {
        if (animator == null) yield break;
        //yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(delay);
        (string, float, List<(string, float, string)>, AnimationClip) animationClip = getClipByName(nameAnimation);
        if (animationClip.Item4 != null)
        {
            if (animationClip.Item4.wrapMode == WrapMode.Once && loop) animationClip.Item4.wrapMode = WrapMode.Loop;
            else if (animationClip.Item4.wrapMode == WrapMode.Loop && !loop) animationClip.Item4.wrapMode = WrapMode.Once;
        }
        //animator.Play(nameAnimation, -1, 0);
        if (animator.speed == 0)
            animator.speed = 1;
        animator.CrossFade(nameAnimation, crossFadeTime, -1, 0);
        animCurrentStatus = nameAnimation;
        if (isNextAnim)
        {
            yield return new WaitForSeconds(animationClip.Item2);
            animationClip = getClipByName(nextAnimation);
            if (animationClip.Item4 != null)
            {
                if (animationClip.Item4.wrapMode == WrapMode.Once && loopNextAnimation) animationClip.Item4.wrapMode = WrapMode.Loop;
                else if (animationClip.Item4.wrapMode == WrapMode.Loop && !loopNextAnimation) animationClip.Item4.wrapMode = WrapMode.Once;
            }
            //animator.Play(nextAnimation, -1, 0);
            animator.CrossFade(nextAnimation, crossFadeTimeNextAnim, -1, 0);
            animCurrentStatus = nextAnimation;
        }
        delaySetAnimCoroutine = null;
    }

    public void setProcessAnim(Animator animator, bool isMainAnimator, string nameAnimation, float delay, float startTime, float crossFadeTime, bool isPauseAnim, float duration)
    {
        if (setProcessAnimCoroutine != null) StopCoroutine(setProcessAnimCoroutine);
        setProcessAnimCoroutine = StartCoroutine(setProcessAnimIEnumerator(animator, isMainAnimator, nameAnimation, delay, startTime, crossFadeTime, isPauseAnim, duration));
    }
    Coroutine setProcessAnimCoroutine = null;
    IEnumerator setProcessAnimIEnumerator(Animator animator, bool isMainAnimator, string nameAnimation, float delay, float startTime, float crossFadeTime, bool isPauseAnim, float duration)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        animator.speed = 1;
        AnimationClip animationClip = getAnimationClipByName(nameAnimation);
        frameProcessCurrent = (int)(startTime * animationClip.frameRate);
        animator.CrossFade(nameAnimation, crossFadeTime, -1, startTime / animationClip.length);
        if (isMainAnimator)
            animCurrentStatus = nameAnimation;
        if (isPauseAnim)
        {
            if (duration > 0)
                yield return new WaitForSeconds(duration);
            //else yield return new WaitForFixedUpdate();
            animator.speed = 0;
        }
        delaySetAnimCoroutine = null;
        yield return new WaitForFixedUpdate();
    }

    public AnimationClip getAnimationClipByName(string animationKey)
    {
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1 == animationKey) return animationClipDurationList[i].Item4;
        }
        return null;
    }

    public float getDurationClipByName(string animationKey)
    {
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1 == animationKey) return animationClipDurationList[i].Item2;
        }
        return -1;
    }

    public (string, float, List<(string, float, string)>, AnimationClip) getClipByName(string animationKey)
    {
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1 == animationKey) return animationClipDurationList[i];
        }
        return ("", -1, new List<(string, float, string)>(), null);
    }

    public float getDurationFirstEventClipByName(string animationKey)
    {
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1 == animationKey && animationClipDurationList[i].Item3.Count > 0) return animationClipDurationList[i].Item3[0].Item2;
        }
        return -1;
    }

    public List<float> getDurationEventClipByName(string animationKey, StaticKeyEnum.EventAnimationKey eventAnimationKey)
    {
        List<float> listEvent = new List<float>();
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1 == animationKey && animationClipDurationList[i].Item3.Count > 0)
                foreach ((string, float, string) e in animationClipDurationList[i].Item3)
                {
                    if (e.Item1 == eventAnimationKey.ToString()) listEvent.Add(e.Item2);
                }
        }
        return listEvent;
    }

    public List<(string, float, string)> getEventClipByName(string animationKey, StaticKeyEnum.EventAnimationKey eventAnimationKey)
    {
        List<(string, float, string)> listEvent = new List<(string, float, string)>();
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1 == animationKey.ToString() && animationClipDurationList[i].Item3.Count > 0)
                foreach ((string, float, string) e in animationClipDurationList[i].Item3)
                {
                    if (e.Item1 == eventAnimationKey.ToString()) listEvent.Add(e);
                }
        }
        return listEvent;
    }

    public List<string> getListSameAnim(string defaultAnim)
    {
        List<string> listAnim = new List<string>();
        for (int i = 0; i < animationClipDurationList.Count; i++)
        {
            if (animationClipDurationList[i].Item1.IndexOf(defaultAnim) == 0 && listAnim.IndexOf(animationClipDurationList[i].Item1) < 0)
            {
                listAnim.Add(animationClipDurationList[i].Item1);
            }
        }
        return listAnim;
    }

    void OnDisable()
    {
        iTween.Stop(this.gameObject, true);
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        iTween.Stop(this.gameObject, true);
        StopAllCoroutines();
    }
}
