using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadMachineScript : MachineScript
{
    public StaticKeyEnum.TypeIngredient typeBread;
    public GameObject breadPrefab;
    public List<IngredientsRecipe> ingredientsRecipes;
    public List<Transform> listElementBreadStackTransform;
    public float cooldownManufacture;
    public List<IngredientScript> breadStack = new List<IngredientScript>();

    void Start()
    {
        breadPrefab = PrefabsList.instance.getIngredientPrefab(PrefabsList.instance.breadDonePrefabs, typeBread.ToString());
        breadStack = new List<IngredientScript>();
        StartCoroutine(cooldownSpawnIngredient());
    }

    public IEnumerator cooldownSpawnIngredient()
    {
        yield return new WaitForFixedUpdate();
        while (true)
        {
            yield return new WaitForSeconds(cooldownManufacture / GameplayController.instance.speedMachines);
            if (breadStack.Count < maxCountStack)
            {
                if (checkIngredientsRecipe())
                {
                    foreach (IngredientsRecipe item in ingredientsRecipes)
                        GameplayController.instance.useIngredientsRecipe(item.typeIngredient, item.countIngredient);
                    IngredientScript ingredientScript = Instantiate(breadPrefab, listElementBreadStackTransform[breadStack.Count].position, Quaternion.identity).GetComponent<IngredientScript>();
                    ingredientScript.transform.eulerAngles = new Vector3(0, 0, 0);
                    breadStack.Add(ingredientScript);
                    UIController.instance.onSound(StaticVar.keySound.sfx_machines);
                }
                if (breadStack.Count < maxCountStack)
                    maxTextPos.gameObject.SetActive(false);
                else maxTextPos.gameObject.SetActive(true);
            }
            else if (!maxTextPos.gameObject.activeSelf)
                maxTextPos.gameObject.SetActive(true);
        }
    }

    bool checkIngredientsRecipe()
    {
        foreach (IngredientsRecipe item in ingredientsRecipes)
        {
            if (item.countIngredient > GameplayController.instance.getIngredientsRecipe(item.typeIngredient).countIngredient)
                return false;
        }
        return true;
    }

    public IngredientScript getIngredient()
    {
        if (breadStack.Count > 0)
        {
            IngredientScript ingredientScript = breadStack[breadStack.Count - 1];
            breadStack.Remove(ingredientScript);
            if (breadStack.Count < maxCountStack)
                maxTextPos.gameObject.SetActive(false);
            else maxTextPos.gameObject.SetActive(true);
            return ingredientScript;
        }
        else return null;
    }
}
