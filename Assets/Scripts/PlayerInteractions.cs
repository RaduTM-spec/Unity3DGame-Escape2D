using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractions : MonoBehaviour
{
    GameObject playerHand = null;
    [SerializeField] float dropPower = 5f;

    //Inventory variables
    int inventoryCapacity = 5;
    GameObject[] inventory = new GameObject[7];

    GameObject objectInHand = null;

    [HideInInspector] public WeaponScript weaponInHandScript = null;
    [HideInInspector] public GrenadeScript grenadeInHandScript = null;
    [HideInInspector] public EquipmentScript equipmentInHandScript = null;
    GameManager gameManager;
    GameObject itemInCollisionWith = null;

  
   
    //-------------------------------------Base Methods------------------------------------//
    private void Awake()
    {
        playerHand = transform.Find("Hand").gameObject;
        gameManager = FindObjectOfType<GameManager>();
        
    }
    void Update()
    {
        ReloadWeapon();
        SwitchItem();
        PickItem(itemInCollisionWith);
        DropItemInHand(false);
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Weapon" || other.tag == "Grenade" || other.tag == "Equiment")
        {
           itemInCollisionWith = other.gameObject;
        }
        else if(other.name == "Button")
        {
            gameManager.CollisionText("Press E to open the Door");
            if(Input.GetKey(KeyCode.E))
            {
                other.GetComponent<DoorButtonScript>().Press();

            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        itemInCollisionWith = null;
        if (collision.name == "Button")
        {
            gameManager.CollisionText("");
        }
    }

    //------------------------------------Interaction Functions------------------------------------//
    #region Interaction Functions Called in Update
    void ReloadWeapon()
    {
        if(Input.GetKeyDown(KeyCode.R) && weaponInHandScript)
        {
            weaponInHandScript.StartCoroutine("Reload");
        }
    }
    void SwitchItem()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
           if(inventory[0] != null && objectInHand != inventory[0])
            {
                PlaceItemInThePlayerBack();
                objectInHand = inventory[0];
                GetScriptOfItemInHand(ref objectInHand);
                PlaceItemInThePlayerHands();
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inventory[1] != null && objectInHand != inventory[1])
            {
                PlaceItemInThePlayerBack();
                objectInHand = inventory[1]; 
                GetScriptOfItemInHand(ref objectInHand);
                PlaceItemInThePlayerHands();
               
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inventory[2] != null && objectInHand != inventory[2])
            {
                PlaceItemInThePlayerBack();
                objectInHand = inventory[2];
                GetScriptOfItemInHand(ref objectInHand);
                PlaceItemInThePlayerHands();
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inventory[3] != null && objectInHand != inventory[3] )
            {
                PlaceItemInThePlayerBack();
                objectInHand = inventory[3];
                GetScriptOfItemInHand(ref objectInHand);
                PlaceItemInThePlayerHands();
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (inventory[4] != null && objectInHand != inventory[4])
            {
                PlaceItemInThePlayerBack();
                objectInHand = inventory[4];
                GetScriptOfItemInHand(ref objectInHand);
                PlaceItemInThePlayerHands();
                

            }
        }
    }
    void PickItem(GameObject item)
    {
        if (!Input.GetKeyDown(KeyCode.E) || !itemInCollisionWith)
            return;
        if (GetNumberOfItemsInInventory() >= inventoryCapacity)
        { 
            gameManager.InventoryIsFull();
            return;
        }
        //Assign object to player
        item.transform.SetParent(transform, true);
        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (inventory[i] == null)
            { 
                inventory[i] = item.gameObject;
                break;
            }
            if(i == inventoryCapacity - 1)
                Debug.Log("Inventory is full");
        }//Add in inventory if possible
        PlaceItemInThePlayerBack();


        objectInHand = item;
        GetScriptOfItemInHand(ref objectInHand);
        AttachItemToPlayer();
        PlaceItemInThePlayerHands();
        
        
        //Positioning the item in hand
        item.transform.position = playerHand.transform.position;
        if(weaponInHandScript)// If is weapon then apply a 90d rotation
             item.transform.rotation = playerHand.transform.rotation * Quaternion.Euler(0,0f,90f);

    }
    public void DropItemInHand(bool specialCase)
    {
        if ((specialCase || Input.GetKeyDown(KeyCode.G)) && objectInHand)
        {
            for (int i = 0; i < inventoryCapacity; i++)
            {
                if (inventory[i] == objectInHand)
                {
                    inventory[i] = null;
                }
            }
            DetachItemFromPlayer();

            ResetScriptOfItemInHandToNULL();
            if(!specialCase)
            PutOtherItemInHandWhenDroppingAnItem();
           
        }
    }
    void PutOtherItemInHandWhenDroppingAnItem()
    {
            //Assign other inventory item if possible
            for (int i = 0; i < inventoryCapacity; i++)
            {
                if (inventory[i] != null)
                {
                     objectInHand = inventory[i];
                     GetScriptOfItemInHand(ref objectInHand);
                     PlaceItemInThePlayerHands();
                     break;
                }
                
            }
    }
    #endregion
    public void Shoot()//This is called Second -> it calls fire/throw functions from the specific weapon
    {
        if(weaponInHandScript)
        {
            weaponInHandScript.Fire();
            return;
        }
        else if(grenadeInHandScript)//&& Input.GetMouseButtonDown(0)
        {
            grenadeInHandScript.Fire();
            return;
        }
    }



    //-------------------------------------UI Methods--------------------------------------------//
  


    //-------------------------------------Mechanical Functions-------------------------------------//
    #region Mechanical Functions
    void GetScriptOfItemInHand(ref GameObject objectInHand)
    {
        weaponInHandScript = objectInHand.GetComponent<WeaponScript>();
        grenadeInHandScript = objectInHand.GetComponent<GrenadeScript>();
        equipmentInHandScript = objectInHand.GetComponent<EquipmentScript>();
    }
    public void ResetScriptOfItemInHandToNULL()
    {
        objectInHand = null;
        weaponInHandScript = null;
        grenadeInHandScript = null;
        equipmentInHandScript = null;
    }
    public bool HasObjectInHand()
    {
        return objectInHand == null? false : true;
    }
    public string GetItemNameInCollisionWith()
    {
        if (itemInCollisionWith)
        {
            return itemInCollisionWith.name;
        }
        else
            return "";
    }
    
    //------------------------------------------Getters------------------------------------------//
    public int GetNumberOfItemsInInventory()
    {
        int num = 0;
        foreach (GameObject item in inventory)
        {
            if (item) num++;
        }
        return num;
    }
    public int GetInventoryCapacity()
    {
        return inventoryCapacity;
    }
    public GameObject[] GetInventory()
    {
        return inventory; 
    }
    public GameObject GetItemInHand()
    {
        return objectInHand;
    }

    //------------------------------------------Setters--------------------------------------------//
    public void RemoveItemFromInventory(GameObject item)
    {
        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (inventory[i] == item)
            {
                inventory[i] = null;
                return;
            }
        }
    }
    #endregion





    #region Current weapon actions
    void AttachItemToPlayer()
    {
        try
        {
            objectInHand.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        catch { }
        if(weaponInHandScript)
        {
            weaponInHandScript.AttachToPlayer();
        }
        else if(grenadeInHandScript)
        {
            grenadeInHandScript.AttachToPlayer();
        }
        else if(equipmentInHandScript)
        {
            equipmentInHandScript.AttachToPlayer();
        }
    }
    void DetachItemFromPlayer()
    {
        try
        {
            objectInHand.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePos - transform.position).normalized;
            objectInHand.GetComponent<Rigidbody2D>().AddForce(Quaternion.Euler(0f,0f,-90f)* direction * dropPower, ForceMode2D.Impulse);
        }
        catch(System.Exception exc)
        {
            Debug.LogException(exc);
        }
        if (weaponInHandScript)
        {
            weaponInHandScript.DetachFromPlayer();
        }
        else if (grenadeInHandScript)
        {
            grenadeInHandScript.DetachFromPlayer();
        }
        else if (equipmentInHandScript)
        {
            equipmentInHandScript.DetachFromPlayer();
        }
    }
    void PlaceItemInThePlayerBack()
    {
        if (weaponInHandScript)
        {
            weaponInHandScript.PlaceInThePlayerBack();
        }
        else if (grenadeInHandScript)
        {
            grenadeInHandScript.PlaceInThePlayerBack();
        }
        else if (equipmentInHandScript)
        {
            equipmentInHandScript.PlaceInThePlayerBack();
        }
    }
    void PlaceItemInThePlayerHands()
    {
        if (weaponInHandScript)
        {
            weaponInHandScript.PlaceInThePlayerHands();
        }
        else if (grenadeInHandScript)
        {
            grenadeInHandScript.PlaceInThePlayerHands();

        }
        else if (equipmentInHandScript)
        {
            equipmentInHandScript.PlaceInThePlayerHands();
        }
    }
    #endregion  
}
