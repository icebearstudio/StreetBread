using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolMoveScript : MonoBehaviour
{
    void Start()
    {

    }

    public void CurveMove(GameObject target, Transform destination, float maxOffset, float time, string type, UnityEvent isDone, float delayTime, bool isDontDestroy = false, bool isRotate = true, bool isDisable = true)
    { // type = "X" cong theo X, type = "Y" cong theo Y
        StartCoroutine(CurveMoveCoroutine(target, destination, maxOffset, time, type, isDone, delayTime, isDontDestroy, isRotate, isDisable));
    }
    private IEnumerator CurveMoveCoroutine(GameObject target, Transform destination, float maxOffset, float time, string type, UnityEvent isDone, float delayTime, bool isDontDestroy = false, bool isRotate = true, bool isDisable = true)
    {
        float moveProgress = 0f;
        yield return new WaitForSeconds(delayTime);
        if (target == null) yield break;
        var startPos = target.transform.position;
        while (moveProgress <= 1.0)
        {
            if (target == null || destination == null) break;
            moveProgress += (Time.deltaTime * (1f + Mathf.Sqrt(moveProgress))) / time;
            if (type == "Y")
            {
                var height = Mathf.Sin(Mathf.PI * moveProgress) * maxOffset;
                if (height < 0f)
                {
                    height = 0;
                }
                Vector2 temp = target.transform.position;
                target.transform.position = Vector3.Lerp(startPos, destination.position, moveProgress) + Vector3.up * height;
                if (isRotate)
                {
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
            else if (type == "X")
            {
                var width = Mathf.Sin(Mathf.PI * moveProgress) * maxOffset;
                if (startPos.x > destination.position.x)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination.position, moveProgress) + Vector3.right * width;
                    if (isRotate)
                    {
                        float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                        target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                    }
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination.position, moveProgress) - Vector3.right * width;
                    if (isRotate)
                    {
                        float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                        target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                    }
                }
                yield return null;
            }
        }
        if (target != null)
            target.transform.position = destination.position;
        if (!isDontDestroy && target != null)
            Destroy(target);
        if (isDone != null)
            isDone.Invoke();
        if (isDisable && target != null && target.activeSelf)
            target.SetActive(false);
    }
}
