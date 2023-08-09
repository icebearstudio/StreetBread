using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class ObjectActivePassive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public UnityEvent OnActive;
    public UnityEvent OnDeactive;

    void OnEnable()
    {
        if (OnActive != null) OnActive.Invoke();
    }

    void OnDisable()
    {
        if (OnDeactive != null) OnDeactive.Invoke();
    }
}
