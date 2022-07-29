using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public enum ButtonState
 {
    red,
    green,
 }
public class DoorButtonScript : MonoBehaviour
{
    [SerializeField] Sprite redButtonSpr = null;
    [SerializeField] Sprite greenButtonSpr = null;
    ButtonState state = ButtonState.red;

    DoorMechanismScript mechanismScript = null;


    private void Awake()
    {
        mechanismScript = transform.parent.gameObject.GetComponent<DoorMechanismScript>();
    }

    public void Press()

    {
        state = ButtonState.green;
        GetComponent<SpriteRenderer>().sprite = greenButtonSpr;
        GetComponent<CircleCollider2D>().enabled = false;
        mechanismScript.OpenDoor();
    }

}
