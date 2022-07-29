using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : ItemScript
{

    [Header("Grenade Characteristics")]
    [SerializeField] float damage = 100f;
    [SerializeField] float reduceSpeedToXPercent = 40f;
    [SerializeField] float explosionPushPower = 10f;
    [SerializeField] float explosionRadius = 20f;
    [SerializeField] float throwPower = 10f;
    [Space]
    [SerializeField] float timeUntilExplosion = 4f;
    [SerializeField] float timeAfterExplosion = 5f;
    float timeOfChainExplosion = 0f; /// <summary>
                                     /// It is set Randomly in Awake Method
                                     /// It represents the time neccesarry to trigger this grenade when other grenade explodes in a relative short radius
                                     /// </summary>


   

    public enum GrenadeState
    {
        wasThrown,
        exploded,
        triggerPinched,
        dropped,
        inHand,
        inInventory,
    }
    public enum GrenadeType
    {
        HE,
        Molotov,
        Smoke,
    }
    public GrenadeType type;
    public GrenadeState state = GrenadeState.dropped;

    //Components
    TrailRenderer thrownGrenadeTrail = null;
    CircleCollider2D circeCollider = null;
    SpriteRenderer grenadeSR = null;
    AudioSource audioSource = null;


    [Header("Sounds")]
    [SerializeField] AudioClip triggerClip;
    [SerializeField] AudioClip throwClip;
    [SerializeField] AudioClip explosionClip;
    [SerializeField] AudioClip afterExplosionClip;

    //Particle Systems
    ParticleSystem throwPS1 = null;/// <summary>
    /// this is Played when the player clicked but not sure if he actually throws and is stopped when it actually throws
    /// </summary>
    ParticleSystem throwPS2 = null;/// <summary>
    /// this is Played when the player surely threw the grenade and is stopped when explosion
    /// </summary>
    ParticleSystem explosionPS1 = null;
    ParticleSystem explosionPS2 = null;
    private void Awake()
    {
        throwPS1 = transform.Find("ThrowPS1").GetComponent<ParticleSystem>();
        throwPS2 = transform.Find("ThrowPS2").GetComponent<ParticleSystem>();
        explosionPS1 = transform.Find("ExplosionPS1").GetComponent<ParticleSystem>();
        explosionPS2 = transform.Find("ExplosionPS2").GetComponent<ParticleSystem>();
        
        thrownGrenadeTrail = GetComponent<TrailRenderer>();
        circeCollider = GetComponent<CircleCollider2D>();
        grenadeSR = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        timeOfChainExplosion = Random.Range(0.1f, .3f);
    }
    
    void Update()
    {
        if(state == GrenadeState.wasThrown)
        {
            if (timeUntilExplosion > 0)
            { 
                timeUntilExplosion -= Time.deltaTime;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = (mousePos - transform.position);
            }
            else if(state != GrenadeState.exploded)
                Explode();

        }
    }
   
   
    //-----------------------------Mechanics----------------------------//
    public override void Fire()
    {
        
        if(state == GrenadeState.inHand)
        {   
            state = GrenadeState.triggerPinched;
            StartCoroutine("Throw");
        }
    }
    IEnumerator Throw()
    {
        //Particle System
        if (throwPS1)
            throwPS1.Play();

        //Sound effect
        audioSource.clip = triggerClip;
        if (audioSource.clip)
            audioSource.Play();

        while(true)
        {
            if (Input.GetMouseButtonUp(0))
                break;
            else     
                 yield return null;
        }
        if (transform.parent == null)//in case was dropped
        {
            state = GrenadeState.dropped;
            if (throwPS1)
                throwPS1.Stop();
            yield break;
        }
        if(transform.parent.GetComponent<PlayerInteractions>().GetItemInHand() != this.gameObject) //in case other item was picked since this was in throwing
        {
            state = GrenadeState.inInventory;
            if (throwPS1)
                throwPS1.Stop();
            if (throwPS2)
                throwPS2.Stop();
            PlaceInThePlayerBack();
            yield break;
        }
            PlayerInteractions parentPlayerScript = transform.parent.GetComponent<PlayerInteractions>();
        if (parentPlayerScript && parentPlayerScript.GetItemInHand() != this.gameObject)// In case which the trigger was pinched but the item in hand was swapped
        {
            GameObject[] inv = parentPlayerScript.GetInventory();
            foreach(GameObject item in inv)
            {
                if(item == this.gameObject)
                {
                    state = GrenadeState.inInventory;
                    if (throwPS1)
                        throwPS1.Stop();
                    yield break;
                }
            }
            state = GrenadeState.dropped;
            if (throwPS1)
                throwPS1.Stop();
            yield break;
        }

        //Preparings
        parentPlayerScript.ResetScriptOfItemInHandToNULL();
        parentPlayerScript.RemoveItemFromInventory(this.gameObject);
        state = GrenadeState.wasThrown;
        circeCollider.radius /= 4;
        thrownGrenadeTrail.enabled = true;

        DetachFromPlayerBcWasThrown();

        //Throw
        //ParticleSystem
        if (throwPS1)
            throwPS1.Stop();
        if(throwPS2)
            throwPS2.Play();

        //Sound Effect
        audioSource.clip = throwClip;
        if (audioSource.clip)
            audioSource.Play();


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = ((Vector2)mousePos - (Vector2)transform.position).normalized;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce((Vector2)direction * throwPower, ForceMode2D.Impulse);
        circeCollider.isTrigger = false;
    } 
    public void Explode()//Explode is called in UPDATE
    {

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        state = GrenadeState.exploded;
        grenadeSR.enabled = false;

        //Particle Effect
        if (throwPS2)
            throwPS2.Stop();
        if(explosionPS1)
        explosionPS1.Play();
        if(explosionPS2)
            explosionPS2.Play();

        //Sound Effect
        audioSource.clip = explosionClip;
        if(audioSource.clip != null)
            audioSource.Play();

        
        //Push Objects and Damage Targets

        if(type == GrenadeType.HE)
        {   
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            
            foreach (Collider2D obj in targets)
        {
            if (obj.gameObject == this.gameObject)
                continue;
            if(type == GrenadeType.HE && obj.CompareTag("Grenade") && obj.GetComponent<GrenadeScript>().state != GrenadeState.exploded)//Chain grenade explosion
            {
                StartCoroutine("TriggerOtherGrenadesWhenExplode", obj.gameObject);
                continue;
            }
            if (obj.CompareTag("Grenade"))
                continue;

            

            Vector3 distance = obj.transform.position - transform.position;
            //Assign Force
            try
            {

                
                obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(1/distance.x,1/distance.y,1/distance.z) * explosionPushPower, ForceMode2D.Impulse);
             
            }
            catch { }

            //Assign damage
            try
            {
                obj.GetComponent<ZombieScript>().DealDamage(damage * explosionRadius/ distance.sqrMagnitude);
                //Get zombie Script and assign damage
            }
            catch { }
        }
        }
        

        StartCoroutine(DestroyGrenadeGameObject(timeAfterExplosion));

        
    }
    IEnumerator DestroyGrenadeGameObject(float delay)
    {
       
        //Audio effect
        AudioSource newAudioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        newAudioSource.clip = afterExplosionClip;
        //for special grenades it can loop
        newAudioSource.Play();

        if (type.Equals(GrenadeType.Molotov))
        {
            while (delay > 0)
            {

                Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                //Push Objects and Damage Targets
                foreach (Collider2D obj in targets)
                {
                    if(obj.CompareTag("Zombie"))
                    {
                        ZombieScript zm = null;
                        try
                        {
                            zm = obj.GetComponent<ZombieScript>();
                        }
                        catch(System.Exception exe)
                        {
                            Debug.LogException(exe);
                        }
                        if(zm != null)
                        {
                            //Assign Slowness
                            zm.SetMovementSpeed(ZombieScript.staticMovementSpeed * reduceSpeedToXPercent/100);
                            //Assign damage
                            zm.DealDamage(damage);
                        }
                        
                    }
                }
                yield return new WaitForSeconds(1f);//It deals damage once per second
                delay-= 1f;
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        

        Destroy(this.gameObject);

    }



    public override void AttachToPlayer()
    {
        circeCollider.enabled = false;
        state = GrenadeState.inHand;
    }
    public override void DetachFromPlayer()
    {
        circeCollider.enabled = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.parent.transform.position.z + 1);
        transform.parent = null;
        state = GrenadeState.dropped;
    }
    public void DetachFromPlayerBcWasThrown()
    {
        circeCollider.enabled = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.parent.transform.position.z + 1);
        transform.parent = null;
        state = GrenadeState.wasThrown;
    }
    public override void PlaceInThePlayerBack()
    {
        grenadeSR.enabled = false;
        state = GrenadeState.inInventory;
    }
    public override void PlaceInThePlayerHands()
    {
        grenadeSR.enabled = true;
        state = GrenadeState.inHand;
    }


    private void OnDrawGizmos()
    {
        if (state == GrenadeState.wasThrown || state.Equals(GrenadeState.exploded))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
    IEnumerator TriggerOtherGrenadesWhenExplode(GameObject grenade)
    {
        GrenadeScript grScript = grenade.GetComponent<GrenadeScript>();
        yield return new WaitForSeconds(grScript.GetTimeOfChainExplosion());
     
        grScript.Explode();
    }

    public float GetTimeOfChainExplosion()
    {
        return timeOfChainExplosion;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(type == GrenadeType.Molotov && state == GrenadeState.wasThrown && (collision.collider.tag == "Wall" || collision.collider.tag == "Obstacle" || collision.collider.CompareTag("Zombie")))
        {
            Explode();

        }
    }
}
