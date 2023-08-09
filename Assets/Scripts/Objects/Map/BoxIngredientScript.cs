using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxIngredientScript : MachineScript
{
    public Transform ingredientMovePos;

    void Start()
    {

    }

    public void moveIngredientToBoxIngredient(IngredientScript ingredientScript, float offset, float time, bool isSoundFx)
    {
        UnityEvent isDone = new UnityEvent();
        isDone.AddListener(() =>
        {
            if (isSoundFx)
                UIController.instance.onSound(StaticVar.keySound.sfx_drop);
            ingredientScript.gameObject.SetActive(false);
            GameplayController.instance.addIngredientsRecipe(ingredientScript.typeIngredient, 1);
            Destroy(ingredientScript.gameObject);
        });
        CurveMove(ingredientScript.gameObject, ingredientMovePos, offset, time, "Y", isDone, 0, true, false, false);
    }
}
