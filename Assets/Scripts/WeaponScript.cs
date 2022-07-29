using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : ItemScript
{
    [Header("Weapon Characteristics")]
    [SerializeField] float damage = 1f;
    [Tooltip("Number of shots/s")] [SerializeField] float fireRate;  float nextTimeFire = 0f;
    [SerializeField] bool hasAutomaticFire = true;//PISTOLS does not have automatic fire
    [SerializeField] int magCapacity = 30;
    [SerializeField] int ammoHoldCapacity = 90;
    [SerializeField] float reloadTime = 2f;
    float firePower = 10f;
    int currentMagAmmo;
    int currentHoldingAmmo;

    //------Weapon state-----//
    public enum WeaponState
    {
        isReloading,
        isFiring,
        isHoldInHand,// + does nothing
        isHoldInInventory,
        isDropped,
    }
    WeaponState state = WeaponState.isDropped;

    //-----Weapon Parts Assignments----/
    GameObject weapon;
    GameObject muzzle;
    GameObject ejectionPort;
    ParticleSystem fireParticleSystem1, fireParticleSystem2;
    GameObject bullet, cartidge;
    SpriteRenderer inHandSprite = null;
    Camera mainCamera;
    GameManager gameManager;

    //---Audio--//
    AudioSource audioSource;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip emptyShootSound;
    [SerializeField] AudioClip reloadSound;
    //------------------Overrided----------------//
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inHandSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        weapon = transform.GetChild(0).gameObject;
        muzzle = weapon.transform.Find("Muzzle").gameObject;


        audioSource = GetComponent<AudioSource>();
        bullet = muzzle.transform.Find("Bullet").gameObject;
        ejectionPort = weapon.transform.Find("EjectionPort").gameObject;
        cartidge = ejectionPort.transform.Find("Cartidge").gameObject;
        fireParticleSystem1 = muzzle.transform.Find("FireParticleSystem").gameObject.GetComponent<ParticleSystem>();
        fireParticleSystem2 = muzzle.transform.Find("FireParticleSystem2").gameObject.GetComponent<ParticleSystem>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        state = WeaponState.isDropped;
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        currentMagAmmo = magCapacity;
        currentHoldingAmmo = ammoHoldCapacity;
    }
    private void Update()
    {
      

    }

    public override void AttachToPlayer()
    {
        GetComponent<CircleCollider2D>().enabled = false;
    
        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;

        state = WeaponState.isHoldInHand;
    }
    public override void DetachFromPlayer()
    {
        //Repositionate it in World Space (weapons are always on 1 on Z axis)
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.parent.transform.position.z + 1);
        transform.parent = null;
        


        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;

        state = WeaponState.isDropped;
    }
    public override void PlaceInThePlayerBack()
    {

        //Debug.Log(this.name + "placed in the back");
        inHandSprite.enabled = false;

        state = WeaponState.isHoldInInventory;
    }
    public override void PlaceInThePlayerHands()
    {

        //Debug.Log(this.name + "placed in the hands");

        nextTimeFire = .2f;
        inHandSprite.enabled = true;

        state = WeaponState.isHoldInHand;
    }
    public override void Fire()
    {
        if(nextTimeFire<=0 && state != WeaponState.isReloading)
        {
            if(currentMagAmmo > 0)
            {
                state = WeaponState.isFiring;
                //DoFireStuff
                fireParticleSystem1.Play();
                fireParticleSystem2.Play();
                float launchAngle = transform.parent.transform.rotation.z;
                Vector2 dir = transform.right;
        

                 GameObject blt = Instantiate(bullet, muzzle.transform.position, Quaternion.Euler(0,0,-90f) * transform.rotation) as GameObject;
                 blt.SetActive(true);
                blt.GetComponent<BulletScript>().SetDamage(this.damage);
                 blt.GetComponent<Rigidbody2D>().AddForce(dir * firePower, ForceMode2D.Impulse);
                 nextTimeFire = 1 / fireRate;

                GameObject ctg = Instantiate(cartidge, ejectionPort.transform.position, Quaternion.Euler(0, 0, -90f) * transform.rotation) as GameObject;
                ctg.SetActive(true);
                 Rigidbody2D ctgRB = ctg.GetComponent<Rigidbody2D>();
                 ctgRB.AddForce(Quaternion.Euler(0,0,-90f) * dir * firePower/13* Random.Range(.8f,1.2f), ForceMode2D.Impulse);

                 ctgRB.AddTorque(Random.Range(-90, -110)/15);
                  
                 currentMagAmmo--;
                if (audioSource.clip != shootSound)
                    audioSource.clip = shootSound;
                 audioSource.Play();
            }
            else
            {
                audioSource.clip = emptyShootSound;
                audioSource.Play();
            }
        }
        else
        {
            if(state != WeaponState.isReloading)
                state = WeaponState.isHoldInHand;
            nextTimeFire -= Time.deltaTime;
        }
    }
    public IEnumerator Reload()
    {
        if (currentMagAmmo == magCapacity)
            yield break;
        state = WeaponState.isReloading;
        if (reloadSound)
        {
            audioSource.clip = reloadSound;
            audioSource.Play();
        }
        gameManager.ReloadingWarning();
       
        yield return new WaitForSeconds(reloadTime);
        if(state == WeaponState.isReloading)
             while(currentMagAmmo < magCapacity && currentHoldingAmmo > 0)
             {
                 currentMagAmmo++;
                 currentHoldingAmmo--;
             }
        state = WeaponState.isHoldInHand;
    }

    //----------------Setters and Getters--------------//
    public int GetCurrentMagAmmo()
    {
        return currentMagAmmo;
    }
    public int GetCurrentHoldingAmmo()
    {
        return currentHoldingAmmo;
    }

    public WeaponState GetWeaponState()
    {
        return state;
    }
}
