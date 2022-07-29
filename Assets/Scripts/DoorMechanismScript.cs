using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanismScript : MonoBehaviour
{
    [SerializeField] float openSpeed = 0.2f;
    float stopTime;
    bool canOpen = false;

    [Header("Sprites")]

    GameObject door1,door2;

    DoorButtonScript buttonScript = null;

    private void Awake()
    {
        buttonScript = transform.Find("Button").gameObject.GetComponent<DoorButtonScript>();
        door1 = transform.Find("Door1").gameObject;
        door2 = transform.Find("Door2").gameObject;
        stopTime = 1 / openSpeed * 4f;
    }
    private void Update()
    {
        if (!canOpen)
            return;
        door1.transform.localPosition = Vector3.Lerp(door1.transform.localPosition, door1.transform.localPosition + new Vector3(0,1,0), openSpeed * Time.deltaTime);
        door2.transform.localPosition = Vector3.Lerp(door2.transform.localPosition, door2.transform.localPosition + new Vector3(0, -1, 0), openSpeed * Time.deltaTime);
        stopTime -= Time.deltaTime;
        if (stopTime < 0)
            this.enabled = false;
    }
    public void OpenDoor()
    {
        canOpen = true;
    }
}
