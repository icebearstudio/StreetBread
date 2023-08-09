using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateTrigger : MonoBehaviour
{
    EventTrigger trigger;
    public Transform rotateTransform;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    float offset = 0.2f;
    public void OnDragDelegate(PointerEventData data)
    {
        rotateTransform.localEulerAngles = new Vector3(0f, rotateTransform.localEulerAngles.y - data.delta.x * offset, 0f);
    }
}
