using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //--------Serializable Variables---------//
    [Header("Player Characteristics")]
    [SerializeField] static public float staticMovementSpeed = 10f;
    float movementSpeed = 10f;
    [SerializeField] float sightRotationSpeed = 12f;
    [Range(0f, 1f)] [SerializeField] float movementDecreaseFactor = 0.6f;


    //--------Debug Private Variables---------//
    bool isAlive = true;
    Vector2 movementDirection = Vector2.zero;
    Vector2 viewDirection = Vector2.zero;
    bool isRotating = false;
    bool isShooting = false;
    PlayerInteractions playerInteractions = null;

    //------------Components------------------//
    [SerializeField] Rigidbody2D rb;

    //--------------Special fields-----------------//
    float velocityFrictionSlowdown = 0.98f;

    //-------------------------------------Overrided Functions------------------------------------//
    private void Awake()
    {
        movementSpeed = staticMovementSpeed;
        playerInteractions = GetComponent<PlayerInteractions>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Movement();
        Sight();
        Shoot();
    }
  

    //------------------------------------Controlling Functions------------------------------------//
    void Movement()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Debug.DrawRay(transform.position, movementDirection, Color.blue);
         // RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, movementDirection, 1f);
        /* foreach (RaycastHit2D hit2D in hits)
           {
            //This foreach might have been used to stop the player to move forward if a wall is in his front
            if(hit2D.collider.CompareTag("Wall"))
                 goto passMovement;
        }*/
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

        //passMovement:
        //Also Rotate when moving
        if(movementDirection != Vector2.zero && !isShooting)
            RotateTo(movementDirection);
        
      
    }
    void Sight()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - transform.position);
        viewDirection = new Vector2(direction.x, direction.y);
        if (movementDirection == Vector2.zero)
        {
            RotateTo(viewDirection);

        }
        Debug.DrawLine(mousePos, transform.position, Color.red);

       
    }
    void Shoot()//This is called First -> it calls Shoot function from Player Interactions
    {
        if (Input.GetMouseButton(0) && playerInteractions.HasObjectInHand())
        {
            if (!isShooting)
            {
                IncreaseMovementSpeed(movementDecreaseFactor);
                isShooting = true;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePos - transform.position);
            viewDirection = new Vector2(direction.x, direction.y);
            RotateTo(viewDirection);

            if (!isRotating)
                playerInteractions.Shoot();   

        }
        else 
        {
            if (isShooting)
            {
                isShooting = false;
                IncreaseMovementSpeed(1/movementDecreaseFactor);
            }
        }
    }

    public void KillPlayer()
    {
        isAlive = false;
        //Debug.Log("Player Dead");
    }




    //-------------------------------------Mechanical Functions-------------------------------------//
    void IncreaseMovementSpeed(float factor)
    {
        movementSpeed *= factor;
    }
    void RotateTo(Vector2 direction)
    {
        
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);

        if(transform.rotation == toRotation)
        {
            isRotating = false;
            return;
        }
        //HERE is a bug, when is shooting righ-up and holding S + D the player rotates towards on the opposite direction like for 359 degrees instead of rotating on the corect side
            transform.rotation = Quaternion.RotateTowards(transform.rotation,toRotation, sightRotationSpeed * Time.deltaTime);
        isRotating = true;
    }

    //----------------getters---------------//
    public bool IsShooting()
    {
        return isShooting;
    }
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }
}
