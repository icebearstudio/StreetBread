using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Events;
using System;

public class IngredientStackScript : ToolMoveScript
{
    public Transform parentStackTransform;
    public List<Transform> listStackElementTransform;
    public float distanceElement;
    public List<IngredientScript> ingredientScriptsStackCurrent = new List<IngredientScript>();
    public int maxStackCount;
    public Transform maxStackText;
    public bool isAvailableGetStack;
    public bool isAvailableBox_Ingredient;
    public bool isSoundFx;

    void Start()
    {
        isAvailableGetStack = true;
        isAvailableBox_Ingredient = true;
    }

    [ContextMenu("createListStackElementTransform")]
    public void createListStackElementTransform()
    {
        listStackElementTransform = new List<Transform>();
        int childCount = parentStackTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform element = parentStackTransform.GetChild(i);
            element.name = "Element_" + i;
            element.localPosition = new Vector3(0, i * distanceElement, 0);
            listStackElementTransform.Add(element);
        }
    }

    public void cooldownGetStack()
    {
        StartCoroutine(cooldownGetStackIEnumerator());
    }

    IEnumerator cooldownGetStackIEnumerator()
    {
        isAvailableGetStack = false;
        yield return new WaitForSeconds(.1f);
        isAvailableGetStack = true;
    }

    public void cooldownBox_Ingredient()
    {
        StartCoroutine(cooldownBox_IngredientIEnumerator());
    }

    IEnumerator cooldownBox_IngredientIEnumerator()
    {
        isAvailableBox_Ingredient = false;
        yield return new WaitForSeconds(.1f);
        isAvailableBox_Ingredient = true;
    }

    public IngredientScript getIngredientStack()
    {
        if (ingredientScriptsStackCurrent.Count > 0)
        {
            IngredientScript ingredientScript = ingredientScriptsStackCurrent[ingredientScriptsStackCurrent.Count - 1];
            ingredientScriptsStackCurrent.Remove(ingredientScript);
            if (maxStackText != null)
            {
                if (ingredientScriptsStackCurrent.Count == maxStackCount)
                {
                    maxStackText.gameObject.SetActive(true);
                    maxStackText.localPosition = new Vector3(maxStackText.localPosition.x, listStackElementTransform[ingredientScriptsStackCurrent.Count - 1].position.y, maxStackText.localPosition.z);
                }
                else
                    maxStackText.gameObject.SetActive(false);
            }
            return ingredientScript;
        }
        else return null;
    }

    public IngredientScript getIngredientStack(StaticKeyEnum.TypeIngredient typeBread)
    {
        if (ingredientScriptsStackCurrent.Count > 0)
        {
            IngredientScript ingredientScript = null;
            foreach (IngredientScript item in ingredientScriptsStackCurrent)
            {
                if (item.typeIngredient == typeBread)
                {
                    ingredientScript = item;
                    ingredientScriptsStackCurrent.Remove(ingredientScript);
                    for (int i = 0; i < ingredientScriptsStackCurrent.Count; i++)
                    {
                        ingredientScriptsStackCurrent[i].transform.parent = listStackElementTransform[i];
                        ingredientScriptsStackCurrent[i].transform.localPosition = Vector3.zero;
                    }
                    break;
                }
            }
            if (maxStackText != null)
            {
                if (ingredientScriptsStackCurrent.Count == maxStackCount)
                {
                    maxStackText.gameObject.SetActive(true);
                    maxStackText.localPosition = new Vector3(maxStackText.localPosition.x, listStackElementTransform[ingredientScriptsStackCurrent.Count - 1].position.y, maxStackText.localPosition.z);
                }
                else
                    maxStackText.gameObject.SetActive(false);
            }
            return ingredientScript;
        }
        else return null;
    }

    public void addIngredientStack(IngredientScript ingredientScript)
    {
        ingredientScriptsStackCurrent.Add(ingredientScript);
        ingredientScript.transform.parent = listStackElementTransform[ingredientScriptsStackCurrent.Count - 1];
        //ingredientScript.transform.localPosition = Vector3.zero;
        moveIngredientToStack(ingredientScript.gameObject, listStackElementTransform[ingredientScriptsStackCurrent.Count - 1], 5, .3f);
        if (maxStackText != null)
        {
            if (ingredientScriptsStackCurrent.Count == maxStackCount)
            {
                maxStackText.gameObject.SetActive(true);
                maxStackText.localPosition = new Vector3(maxStackText.localPosition.x, listStackElementTransform[ingredientScriptsStackCurrent.Count - 1].position.y, maxStackText.localPosition.z);
            }
            else
                maxStackText.gameObject.SetActive(false);
        }
    }

    public void moveIngredientToStack(GameObject ingredientGameObject, Transform stackPos, float offset, float time)
    {
        UnityEvent isDone = new UnityEvent();
        isDone.AddListener(() =>
        {
            ingredientGameObject.transform.localPosition = Vector3.zero;
            ingredientGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            if (isSoundFx)
                UIController.instance.onSound(StaticVar.keySound.sfx_pick);
        });
        CurveMove(ingredientGameObject, stackPos, offset, time, "Y", isDone, 0, true, false, false);
    }

    public void resetMaxText()
    {
        if (ingredientScriptsStackCurrent.Count == maxStackCount)
        {
            maxStackText.gameObject.SetActive(true);
            maxStackText.localPosition = new Vector3(maxStackText.localPosition.x, listStackElementTransform[ingredientScriptsStackCurrent.Count - 1].position.y, maxStackText.localPosition.z);
        }
        else
            maxStackText.gameObject.SetActive(false);
    }
}
