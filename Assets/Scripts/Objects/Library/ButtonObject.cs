using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonObject : MonoBehaviour
{
    Renderer cubeRenderer;

    void Start()
    {
        cubeRenderer = gameObject.GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
    }

    void OnMouseUp()
    {
    }

    void OnMouseDrag()
    {

    }

    void OnMouseEnter()
    {
    }

    void OnMouseExit()
    {
    }

    void OnMouseOver()
    {

    }

    void OnMouseUpAsButton()
    {
        if (onClick != null)
            onClick.Invoke();
    }

    [System.Serializable]
    public class OnClick : UnityEvent
    { };

    public OnClick onClick;
}
