using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickCharacterMapHouse : MonoBehaviour
{
    public static JoystickCharacterMapHouse instance;
    public bool isMove;
    public Rigidbody targetMoveRigidbody;
    public CharacterMap characterMap;
    public Transform rotateCharacterTransform;
    public DynamicJoystick dynamicJoystick;
    float speedMove = 6f;
    public float SpeedMove { get => speedMove; set => speedMove = value; }
    public IngredientStackScript ingredientStackScript;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        isMove = true;
    }

    void FixedUpdate()
    {
        if (isMove)
            move();
        else
            stopMove();
    }

    public Vector2 directionDynamicJoystick = Vector2.zero;
    void move()
    {
        if (dynamicJoystick.Direction.magnitude > 0.3f)
        {
            if (directionDynamicJoystick.magnitude == 0)
            {
                if (characterMap != null && characterMap.gameObject.activeInHierarchy)
                {
                    if (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0)
                        characterMap.characterAnimScript.setAnim(StaticKeyEnum.AnimKey.Run.ToString(), true, 0, 0.1f);
                    else
                        characterMap.characterAnimScript.setAnim(StaticKeyEnum.AnimKey.Run_Carry.ToString(), true, 0, 0.1f);
                }
            }
            {
                if (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 && characterMap.characterAnimScript.animCurrentStatus == StaticKeyEnum.AnimKey.Run_Carry.ToString())
                    characterMap.characterAnimScript.setAnim(StaticKeyEnum.AnimKey.Run.ToString(), true, 0, 0.1f);
                else if (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && characterMap.characterAnimScript.animCurrentStatus == StaticKeyEnum.AnimKey.Run.ToString())
                    characterMap.characterAnimScript.setAnim(StaticKeyEnum.AnimKey.Run_Carry.ToString(), true, 0, 0.1f);
            }
            directionDynamicJoystick = dynamicJoystick.Direction;
            targetMoveRigidbody.velocity = SpeedMove * new Vector3(directionDynamicJoystick.x, 0, directionDynamicJoystick.y).normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, targetMoveRigidbody.velocity, Vector3.up);
            rotateCharacterTransform.localEulerAngles = Vector3.LerpUnclamped(rotateCharacterTransform.localEulerAngles, new Vector3(0, angle, 0), 1f);
        }
        else stopMove();
    }

    public void stopMove()
    {
        if (directionDynamicJoystick.magnitude > 0)
        {
            targetMoveRigidbody.velocity = Vector3.zero;
            directionDynamicJoystick = Vector2.zero;
            if (characterMap != null && characterMap.characterAnimScript.gameObject.activeInHierarchy && characterMap.characterAnimScript.animCurrentStatus != StaticKeyEnum.AnimKey.Idle.ToString() && characterMap.characterAnimScript.animCurrentStatus != StaticKeyEnum.AnimKey.Idle_Carry.ToString())
            {
                if (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0)
                    characterMap.characterAnimScript.setAnim(StaticKeyEnum.AnimKey.Idle.ToString(), true, 0, 0.05f);
                else
                    characterMap.characterAnimScript.setAnim(StaticKeyEnum.AnimKey.Idle_Carry.ToString(), true, 0, 0.05f);
            }
        }
    }
}
