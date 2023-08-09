using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MachineScript : ToolMoveScript
{
    public bool isUnlock;
    public StaticKeyEnum.TypeMachine typeMachine;
    public Transform maxTextPos;
    public int maxCountStack;
    public Transform colliderTargetMovePos;

    void Start()
    {

    }
}
