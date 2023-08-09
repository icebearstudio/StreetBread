using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class SkillsGun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private IEnumerator activeJumpCoroutine;
    public float JumpProgress { get; private set; }
    public bool isStartJump;
    public void Jump(GameObject target, Vector3 destination, float maxOffset, float time, int type, UnityEvent e, float delayTime = 0f, bool isDontDestroy = false)
    {
        if (activeJumpCoroutine != null)
        {
            StopCoroutine(activeJumpCoroutine);
            activeJumpCoroutine = null;
            JumpProgress = 0.0f;
        }
        activeJumpCoroutine = JumpCoroutine(target, destination, maxOffset, time, type, e, delayTime, isDontDestroy);
        if (!isStartJump)
        {
            isStartJump = true;
        }
        StartCoroutine(activeJumpCoroutine);
    }
    private IEnumerator JumpCoroutine(GameObject target, Vector3 destination, float maxOffset, float time, int type, UnityEvent e, float delayTime, bool isDontDestroy = false)
    {
        yield return new WaitForSeconds(delayTime);
        var startPos = target.transform.position;
        while (JumpProgress <= 1.0)
        {
            JumpProgress += (Time.deltaTime * (1f + Mathf.Sqrt(JumpProgress))) / time;
            if (type == 0)
            {
                var height = Mathf.Sin(Mathf.PI * JumpProgress) * maxOffset;
                if (height < 0f)
                {
                    height = 0;
                }
                if (startPos.y > destination.y)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress) - Vector3.up * height;
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress) + Vector3.up * height;
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
            else
            {
                var width = Mathf.Sin(Mathf.PI * JumpProgress) * maxOffset;
                if (width < 0f)
                {
                    width = 0;
                }
                if (startPos.x > destination.x)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress) + Vector3.right * width;
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress) - Vector3.right * width;
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
        }
        isStartJump = false;
        target.transform.position = destination;
        if (e != null)
        {
            try
            {
                e.Invoke();
            }
            catch (Exception exception) { print(exception); }
        }
        if (!isDontDestroy)
            Destroy(target);
    }

    private IEnumerator activeThrowCoroutine;
    public float ThrowProgress { get; private set; }
    public bool isStartThrow;
    public void Throw(GameObject target, Vector3 destination, float maxOffset, float time, UnityEvent e)
    {
        if (activeThrowCoroutine != null)
        {
            StopCoroutine(activeThrowCoroutine);
            activeThrowCoroutine = null;
            ThrowProgress = 0.0f;
        }
        activeThrowCoroutine = ThrowCoroutine(target, destination, maxOffset, time, e);
        if (!isStartThrow)
        {
            isStartThrow = true;
        }
        StartCoroutine(activeThrowCoroutine);
    }

    private IEnumerator ThrowCoroutine(GameObject target, Vector3 destination, float maxOffset, float time, UnityEvent e)
    {
        var startPos = target.transform.position;
        Vector3 tempScale = target.transform.localScale;
        while (ThrowProgress <= 1.0)
        {
            if (ThrowProgress <= 0.5f)
            {
                ThrowProgress += (Time.deltaTime * (1f + ThrowProgress * ThrowProgress)) / time;
            }
            else if (ThrowProgress >= 0.5f)
            {
                ThrowProgress += (Time.deltaTime * (1f + Mathf.Sqrt(ThrowProgress - 0.5f))) / time;
            }
            Vector2 temp = target.transform.position;
            target.transform.position = Vector3.Lerp(startPos, destination, ThrowProgress);
            if (ThrowProgress < 0.5f)
            {
                target.transform.localScale = tempScale * (1f + maxOffset * ThrowProgress * 2);
            }
            else
            {
                target.transform.localScale = tempScale * (1f + maxOffset - maxOffset * (ThrowProgress - 0.5f) * 2);
            }
            yield return null;
        }
        target.transform.localScale = tempScale;
        isStartThrow = false;
        target.transform.position = destination;
        if (e != null)
        {
            try
            {
                e.Invoke();
            }
            catch (Exception exception) { print(exception); }
        }
        Destroy(target);
    }
}
