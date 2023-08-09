using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffScript : CharacterAnimScript
{
    public StaticKeyEnum.TypeStaff typeStaff;
    public List<MachineScript> startMachineAction;
    public List<MachineScript> endMachineAction;
    public CounterMachineScript counterMachineScriptTarget;
    public IngredientStackScript ingredientStackScript;
    float speedMove = 2.5f;
    public bool isMoving;

    new void Start()
    {
        base.Start();
    }

    public void setupAction()
    {
        if (typeStaff == StaticKeyEnum.TypeStaff.Cashier_Staff)
        {
            counterMachineScriptTarget.staffSellingScript = this;
            counterMachineScriptTarget.isRentStaffCashier = true;
            transform.position = counterMachineScriptTarget.posSellingAvailable.position;
        }
        else if (typeStaff == StaticKeyEnum.TypeStaff.Luggage_Staff)
        {
            transform.position = startMachineAction[0].colliderTargetMovePos.position;
            directionCurrent = StaffDirectionMove.GoToTarget;
            StartCoroutine(moveAction_Luggage_Staff());
        }
        else if (typeStaff == StaticKeyEnum.TypeStaff.Ingredient_Staff)
        {
            transform.position = startMachineAction[0].colliderTargetMovePos.position;
            directionCurrent = StaffDirectionMove.GoToTarget;
            StartCoroutine(moveAction_Ingredient_Staff());
        }
    }

    enum StaffDirectionMove
    {
        GoToTarget, BackBeginning
    }

    StaffDirectionMove directionCurrent;
    public IEnumerator moveAction_Luggage_Staff()
    {
        int tartgetIndexCurrent = 0;
        yield return new WaitForFixedUpdate();
        while (true)
        {
            speedMove = StaticKeyEnum.defaultStaff_SpeedMove * GameplayController.instance.speedMachines;
            if (directionCurrent == StaffDirectionMove.GoToTarget)
            {
                if (((BreadMachineScript)endMachineAction[tartgetIndexCurrent]).isUnlock)
                {
                    typeIngredientTarget = ((BreadMachineScript)endMachineAction[tartgetIndexCurrent]).typeBread;
                    Transform posMove = endMachineAction[tartgetIndexCurrent].colliderTargetMovePos;
                    float timeMove = (posMove.position - transform.position).magnitude / speedMove;
                    iTween.MoveTo(gameObject, posMove, timeMove, iTween.EaseType.linear);
                    isMoving = true;
                    updateAnim();
                    if ((posMove.position - transform.position).normalized != Vector3.zero)
                        transform.forward = (posMove.position - transform.position).normalized;
                    yield return new WaitForSeconds(timeMove);
                    isMoving = false;
                    yield return new WaitForFixedUpdate();
                    updateAnim();
                }
                yield return new WaitForSeconds(1f);
                directionCurrent = StaffDirectionMove.BackBeginning;
            }
            else if (directionCurrent == StaffDirectionMove.BackBeginning)
            {
                Transform posMove = null;
                foreach (MachineScript item in startMachineAction)
                {
                    if (((CounterMachineScript)item).isUnlock && ((CounterMachineScript)item).typeBread == typeIngredientTarget)
                        posMove = item.colliderTargetMovePos;
                }
                if (posMove != null)
                {
                    float timeMove = (posMove.position - transform.position).magnitude / speedMove;
                    iTween.MoveTo(gameObject, posMove, timeMove, iTween.EaseType.linear);
                    isMoving = true;
                    updateAnim();
                    if ((posMove.position - transform.position).normalized != Vector3.zero)
                        transform.forward = (posMove.position - transform.position).normalized;
                    yield return new WaitForSeconds(timeMove);
                    isMoving = false;
                    yield return new WaitForFixedUpdate();
                    updateAnim();
                    yield return new WaitForSeconds(1f);
                    directionCurrent = StaffDirectionMove.GoToTarget;
                    tartgetIndexCurrent = (tartgetIndexCurrent + 1) % endMachineAction.Count;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    StaticKeyEnum.TypeIngredient typeIngredientTarget = StaticKeyEnum.TypeIngredient.None;
    public IEnumerator moveAction_Ingredient_Staff()
    {
        int tartgetIndexCurrent = 0;
        yield return new WaitForFixedUpdate();
        while (true)
        {
            speedMove = StaticKeyEnum.defaultStaff_SpeedMove * GameplayController.instance.speedMachines;
            if (directionCurrent == StaffDirectionMove.GoToTarget)
            {
                if (((ShelveIngredientScript)endMachineAction[tartgetIndexCurrent]).isUnlock)
                {
                    typeIngredientTarget = ((ShelveIngredientScript)endMachineAction[tartgetIndexCurrent]).typeIngredient;
                    Transform posMove = endMachineAction[tartgetIndexCurrent].colliderTargetMovePos;
                    float timeMove = (posMove.position - transform.position).magnitude / speedMove;
                    iTween.MoveTo(gameObject, posMove, timeMove, iTween.EaseType.linear);
                    isMoving = true;
                    updateAnim();
                    if ((posMove.position - transform.position).normalized != Vector3.zero)
                        transform.forward = (posMove.position - transform.position).normalized;
                    yield return new WaitForSeconds(timeMove);
                    isMoving = false;
                    yield return new WaitForFixedUpdate();
                    updateAnim();
                }
                yield return new WaitForSeconds(1f);
                directionCurrent = StaffDirectionMove.BackBeginning;
            }
            else if (directionCurrent == StaffDirectionMove.BackBeginning)
            {
                Transform posMove = startMachineAction[0].colliderTargetMovePos;
                float timeMove = (posMove.position - transform.position).magnitude / speedMove;
                iTween.MoveTo(gameObject, posMove, timeMove, iTween.EaseType.linear);
                isMoving = true;
                updateAnim();
                if ((posMove.position - transform.position).normalized != Vector3.zero)
                    transform.forward = (posMove.position - transform.position).normalized;
                yield return new WaitForSeconds(timeMove);
                isMoving = false;
                yield return new WaitForFixedUpdate();
                updateAnim();
                yield return new WaitForSeconds(1f);
                directionCurrent = StaffDirectionMove.GoToTarget;
                tartgetIndexCurrent = (tartgetIndexCurrent + 1) % endMachineAction.Count;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void updateAnim()
    {
        if (isMoving)
        {
            if (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 && animCurrentStatus != StaticKeyEnum.AnimKey.Walk.ToString())
                setAnim(StaticKeyEnum.AnimKey.Walk.ToString(), true, 0, 0.1f);
            else if (animCurrentStatus != StaticKeyEnum.AnimKey.Walk_Carry.ToString())
                setAnim(StaticKeyEnum.AnimKey.Walk_Carry.ToString(), true, 0, 0.1f);
        }
        else
        {
            if (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 && animCurrentStatus != StaticKeyEnum.AnimKey.Idle.ToString())
                setAnim(StaticKeyEnum.AnimKey.Idle.ToString(), true, 0, 0.1f);
            else if (animCurrentStatus != StaticKeyEnum.AnimKey.Idle_Carry.ToString())
                setAnim(StaticKeyEnum.AnimKey.Idle_Carry.ToString(), true, 0, 0.1f);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (typeStaff == StaticKeyEnum.TypeStaff.Ingredient_Staff && collider.tag == "IngredientCollider" && !isMoving)
        {
            if (ingredientStackScript.maxStackCount > ingredientStackScript.ingredientScriptsStackCurrent.Count && ingredientStackScript.isAvailableGetStack
            && (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 || (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadNormal_Done && ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadPlus_Done))))
            {
                ShelveIngredientScript shelveIngredientScript = collider.GetComponentInParent<ShelveIngredientScript>();
                if (shelveIngredientScript.listIngredient.Count > 0 && shelveIngredientScript.typeIngredient == typeIngredientTarget)
                {
                    ingredientStackScript.cooldownGetStack();
                    ingredientStackScript.addIngredientStack(shelveIngredientScript.getIngredient());
                    updateAnim();
                }
            }
        }
        else if (typeStaff == StaticKeyEnum.TypeStaff.Ingredient_Staff && collider.tag == "Box_Ingredient" && !isMoving)
        {
            if (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && ingredientStackScript.isAvailableBox_Ingredient
            && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadNormal_Done && ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadPlus_Done))
            {
                BoxIngredientScript boxIngredientScript = collider.GetComponentInParent<BoxIngredientScript>();
                ingredientStackScript.cooldownBox_Ingredient();
                IngredientScript ingredientScript = ingredientStackScript.getIngredientStack();
                ingredientScript.transform.parent = GameplayController.instance.draftPos;
                boxIngredientScript.moveIngredientToBoxIngredient(ingredientScript, 3f, .3f, false);
                updateAnim();
            }
        }
        else if (typeStaff == StaticKeyEnum.TypeStaff.Luggage_Staff && collider.tag == "GetBreadCollider" && !isMoving)
        {
            if (ingredientStackScript.maxStackCount > ingredientStackScript.ingredientScriptsStackCurrent.Count && ingredientStackScript.isAvailableGetStack
            && (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 || (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadNormal_Done || ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadPlus_Done))))
            {
                BreadMachineScript breadMachineScript = collider.GetComponentInParent<BreadMachineScript>();
                if (breadMachineScript.breadStack.Count > 0)
                {
                    ingredientStackScript.cooldownGetStack();
                    ingredientStackScript.addIngredientStack(breadMachineScript.getIngredient());
                    updateAnim();
                }
            }
        }
        else if (typeStaff == StaticKeyEnum.TypeStaff.Luggage_Staff && collider.tag == "GetBreadCountMachine" && !isMoving)
        {
            if (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && ingredientStackScript.isAvailableBox_Ingredient
            && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadNormal_Done || ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadPlus_Done))
            {
                CounterMachineScript counterMachineScript = collider.GetComponentInParent<CounterMachineScript>();
                if (counterMachineScript.maxCountStack > counterMachineScript.breadStackCurrent.Count)
                {
                    IngredientScript ingredientScript = ingredientStackScript.getIngredientStack(counterMachineScript.typeBread);
                    if (ingredientScript != null)
                    {
                        ingredientStackScript.cooldownBox_Ingredient();
                        counterMachineScript.breadStackCurrent.Add(ingredientScript);
                        Transform parent = counterMachineScript.listStackElementTransform[counterMachineScript.breadStackCurrent.Count - 1];
                        ingredientScript.transform.parent = parent;
                        counterMachineScript.moveIngredientToCounterMachine(ingredientScript, parent, 3f, .3f, false);
                        updateAnim();
                    }
                }
            }
        }
    }
}
