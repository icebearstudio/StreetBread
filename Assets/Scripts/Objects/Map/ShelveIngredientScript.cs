using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelveIngredientScript : MachineScript
{
    public StaticKeyEnum.TypeIngredient typeIngredient;
    public GameObject ingredientPrefab;
    public float cooldownTime;
    public List<Transform> listElementTransform;
    public List<IngredientScript> listIngredient;

    void Start()
    {
        ingredientPrefab = PrefabsList.instance.getIngredientPrefab(PrefabsList.instance.ingredientPrefabs, typeIngredient.ToString());
        listIngredient = new List<IngredientScript>();
        StartCoroutine(cooldownSpawnIngredient());
    }

    public IEnumerator cooldownSpawnIngredient()
    {
        yield return new WaitForFixedUpdate();
        while (true)
        {
            yield return new WaitForSeconds(cooldownTime / GameplayController.instance.speedMachines);
            if (listIngredient.Count < maxCountStack)
            {
                IngredientScript ingredientScript = Instantiate(ingredientPrefab, listElementTransform[listIngredient.Count].position, Quaternion.identity).GetComponent<IngredientScript>();
                ingredientScript.transform.eulerAngles = new Vector3(0, 0, 0);
                listIngredient.Add(ingredientScript);
                if (listIngredient.Count < maxCountStack)
                    maxTextPos.gameObject.SetActive(false);
                else maxTextPos.gameObject.SetActive(true);
            }
            else if (!maxTextPos.gameObject.activeSelf)
                maxTextPos.gameObject.SetActive(true);
        }
    }

    public IngredientScript getIngredient()
    {
        if (listIngredient.Count > 0)
        {
            IngredientScript ingredientScript = listIngredient[listIngredient.Count - 1];
            listIngredient.Remove(ingredientScript);
            if (listIngredient.Count < maxCountStack)
                maxTextPos.gameObject.SetActive(false);
            else maxTextPos.gameObject.SetActive(true);
            return ingredientScript;
        }
        else return null;
    }
}
