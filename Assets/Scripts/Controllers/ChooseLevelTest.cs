using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseLevelTest : MonoBehaviour
{
    public static ChooseLevelTest instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
    }
}
