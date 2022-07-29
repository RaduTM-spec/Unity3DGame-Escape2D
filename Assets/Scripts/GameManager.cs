using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class GameManager : MonoBehaviour
{
    List<GameObject> playersList = new List<GameObject>();
    List<PlayerInteractions> playersInteractions = new List<PlayerInteractions>();




    //------UI--------//
    [Space, Header("UI")]
    [Header("Canvas Childs")]
    [SerializeField] TMPro.TMP_Text ammoText;
    [SerializeField] TMPro.TMP_Text collisionText;
    [SerializeField] TMPro.TMP_Text inventoryFull; float fadeTime = 2f;
    [SerializeField] TMPro.TMP_Text inventoryText;
    [SerializeField] TMPro.TMP_Text performanceStatistics;[SerializeField] float updatePerfStatsRate = 4f; bool canUpdatePerfStatsRate = true;   


    [SerializeField] Image ammoImage;
    [SerializeField] Image reloadWarningImage; float reloadWarningImageShowRate = 2f; bool reloadWarningImageIsPlaying = false;


    [Space, Header("Sprites")]
    [SerializeField] Sprite bulletsImage;
    [SerializeField] Sprite HEImage;
    [SerializeField] Sprite molotovImage;

    //-----------------------------------------Base Methods---------------------------------//
    private void Awake()
    {
        //Assign Players
        GameObject []playersArray = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playersArray)
        {
            playersList.Add(player);
        }
        //Assign Scripts
        for (int i = 0; i < playersList.Count; i++)
        {
            try
            {
                playersInteractions.Add(playersList[i].GetComponent<PlayerInteractions>());
            }
            catch 
            {

               
            }
        }
    }
    void Start()
    {
        
    }
    private void Update()
    {
        if (canUpdatePerfStatsRate)
            StartCoroutine(PerformanceStatistics(updatePerfStatsRate));
    }
    private void LateUpdate()
    {
        UpdateUI();
        
        
    }



    void UpdateUI()
    {
        
        for (int i = 0; i < playersInteractions.Count; i++)
        {
            //Ammo Display - Right Down corner
            if (playersInteractions[i].weaponInHandScript)
            {
            ammoImage.color = Color.white;
            ammoText.text = playersInteractions[i].weaponInHandScript.GetCurrentMagAmmo().ToString() + "/" + playersInteractions[i].weaponInHandScript.GetCurrentHoldingAmmo().ToString();
            ammoImage.sprite = bulletsImage;
            }
            else if (playersInteractions[i].grenadeInHandScript)
            {
            ammoImage.color = Color.white;
            ammoText.text = "1";
                if (playersInteractions[i].grenadeInHandScript.type == GrenadeScript.GrenadeType.HE)
                    ammoImage.sprite = HEImage;
                else if (playersInteractions[i].grenadeInHandScript.type == GrenadeScript.GrenadeType.Molotov)
                    ammoImage.sprite = molotovImage;
            }
            else if (playersInteractions[i].equipmentInHandScript)
             {
            ammoImage.color = Color.white;
            }
            else
            {
            ammoText.text = "";
            ammoImage.color = Color.clear;
            ammoImage.sprite = null;
            }

            //Reload Check
            if (playersInteractions[i].weaponInHandScript != null && playersInteractions[i].weaponInHandScript.GetWeaponState() == WeaponScript.WeaponState.isReloading)
            {
                reloadWarningImage.enabled = true;
            }
            else reloadWarningImage.enabled = false;


            //collision check
            string itemInCollisionName = playersInteractions[i].GetItemNameInCollisionWith();
            if (itemInCollisionName != "" && playersInteractions[i].GetNumberOfItemsInInventory() < playersInteractions[i].GetInventoryCapacity())
            {
                StringBuilder str = new StringBuilder();
                str.Append("Press E to pick ");
                str.Append(itemInCollisionName);
                CollisionText(str.ToString());
               
            }
            else if (collisionText.text.Substring(0, 5) == "Press")
                collisionText.enabled = false;
            
          

            //Update Inventory
            UpdateInventoryText(playersInteractions[i]);

        
        }
       
        
        
    }
    public void CollisionText(string text)
    {

        if (text == "")
            collisionText.enabled = false;
        else
        {
            collisionText.enabled = true;
            collisionText.text = text;
            return;
        }
        

    }


    private void UpdateInventoryText(PlayerInteractions playerInteract)
    {
       
        string invText = "";
        int invCap = playerInteract.GetInventoryCapacity();
        GameObject[] inventory = playerInteract.GetInventory();
        GameObject itemInHand = playerInteract.GetItemInHand();
        for (int i = 0; i < invCap; i++)
        {
          
                if (inventory[i])
                {
                    if (itemInHand && itemInHand.name == inventory[i].name)
                        invText += "<color=#66a3ff><b><size=130%>";

                    invText += inventory[i].name + " - ";
                    
                }
                else
                    invText += "<color=grey></b><size=100%>";

                invText += (i+1).ToString();
         



            invText += "<color=white></b><size=100%>";
            invText += System.Environment.NewLine;
        }

        inventoryText.text = invText;
    }
    public void InventoryIsFull()
    {
        inventoryFull.enabled = true;
        inventoryFull.color = new Color(inventoryFull.color.r, inventoryFull.color.g, inventoryFull.color.b, 1.0f);
        StartCoroutine(FadeText(inventoryFull,fadeTime));
    }
    public void ReloadingWarning()
    {
        ;//Must Be implemented and called
    }

    //-----------------IEnumerators--------------//
    IEnumerator FadeText(TMPro.TMP_Text text, float time)
    {
        while (text.color.a > .0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime) / time);
            yield return null;
        }
        text.enabled = false;
    }
    IEnumerator PerformanceStatistics(float rate)
    {
        canUpdatePerfStatsRate = false;
        yield return new WaitForSeconds(1/rate);
        performanceStatistics.text = (1 / Time.deltaTime).ToString("0") + " FPS";
        canUpdatePerfStatsRate = true;
    }

    IEnumerator ZeroToOneAlphaImage()
    {
        yield return null;
    }
    IEnumerator OneToZeroAlphaImage()
    {
        yield return null;
    }
}
