using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    //Characteristics
    static public float staticMovementSpeed = 6f;
    [SerializeField] float hitpoints = 100f;
    [SerializeField] float movementSpeed = 10f;
    public bool searchCommand = true;
    public float stunned = 0f;
    [SerializeField] float movementSpeedRegainPercent50tps = 4f;
    float delayAfterDead = 3f;

    //Targeting
    List<Transform> targets = new List<Transform>();
    Transform nearestTarget;
    SimpleFollowingScript simpleFollowingScript = null;

    //HP Bar
    GameObject hpBar = null;


    [Header("Sounds")]
    AudioSource audioSource;
    [SerializeField] AudioClip getDamageSound;
    [SerializeField] AudioClip dieSound;

    private void Awake()
    {
        simpleFollowingScript = GetComponent<SimpleFollowingScript>();
        hpBar = transform.Find("HPBar").gameObject;
        audioSource = GetComponent<AudioSource>();

        CollectTargets();
    }
    private void Update()
    {
        if(hitpoints < 0)
        {
            transform.parent = null;
            StartCoroutine(Die(delayAfterDead));
        }
        nearestTarget = GetNearestTarget(targets);
        if (stunned > 0f)
            stunned -= Time.deltaTime;
       
    }
    private void FixedUpdate()
    {
        if(movementSpeed < staticMovementSpeed)
        {
            movementSpeed += movementSpeedRegainPercent50tps/100 * movementSpeed;
        }
    }
    private void LateUpdate()
    {
        UpdateHPBar();
    }

    void CollectTargets()
    {
        GameObject[] targ = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject gameObject in targ)
        {
            targets.Add(gameObject.transform);
        }
    }
    void UpdateHPBar()
    {
        hpBar.transform.localScale = new Vector3(.5f * hitpoints / 100f, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
    }
    Transform GetNearestTarget(List<Transform> list)
    {
        Transform closestTarget = list[0];
        float distance = (transform.position - list[0].transform.position).magnitude;

        for (int i = 1; i < list.Count; i++)
        {
            float newDistance = (transform.position - list[0].transform.position).magnitude;
            if (newDistance < distance)
            {
                distance = newDistance;
                closestTarget = list[i];
            }
        }

        return closestTarget;

    }
    public Transform GetNearestTargetVar()
    {
        return nearestTarget;
    }
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }



    IEnumerator Die(float delay)
    {
        searchCommand = false;
        audioSource.PlayOneShot(dieSound);

        //Transparency
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a / 2);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        yield return delay;
        transform.parent = null;
        Destroy(this.gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            try
            {
                searchCommand = false;
                collision.collider.GetComponent<PlayerController>().KillPlayer();
            }
            catch
            {
                Debug.LogError("PlayerController Script couldn't be accesed for killing purpose");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
                searchCommand = true;
    }


    //---------------------setters and getters---------------------//
    public void SetMovementSpeed(float set)
    {
        movementSpeed = set;
    }
    public void DealDamage(float damage)
    {
        hitpoints -= damage;
    }

}
