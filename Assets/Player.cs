using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public float playerSpeed = 5f;
    public float waitTime = 0.5f;
    public float turnSpeed = 7; 
    public float smoothMoveTime = .3f;

    float smoothInputMagnitude;
    float smoothMoveVelocity;
    float turn;
    Vector3 velocity;
    float toEndDistance;

    new Rigidbody rigidbody;

    bool playerSpotted;

    public static event System.Action OnReachingEnd;
    public GameObject End;


    void Start()
    {
        GameObject end = End;
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardSpotted += PlayerSpotted;
    }
    void Update()
    {
        //Checking for End
        Vector3 diff = new Vector3 (transform.position.x - End.transform.position.x, 0, transform.position.z - End.transform.position.z);
        toEndDistance = diff.sqrMagnitude;
        //stopping player
        Vector3 input = Vector3.zero;
        velocity = input;
        //controllers
        if (!playerSpotted)
        { 
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = input.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
       
        //moving crosswward
        input *= (Mathf.Abs(input.x) == 1) && (Mathf.Abs(input.z) == 1) ? .7f : 1;
        //turning player
        float turnAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
        turn = Mathf.LerpAngle(turn, turnAngle, turnSpeed * Time.deltaTime * inputMagnitude);
        //velocity
        velocity = transform.forward * playerSpeed * smoothInputMagnitude;
        }
    }
    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * turn));
        rigidbody.MovePosition(rigidbody.position + (velocity * Time.fixedDeltaTime));
    }
    void PlayerSpotted()
    {
        playerSpotted = true;
    }

    void OnDestroy()
    {
        Guard.OnGuardSpotted -= PlayerSpotted;
    }

    void OnTriggerStay(Collider hitCollider)
    {
        if (hitCollider.CompareTag("End") && (toEndDistance < 0.3))
        {
            PlayerSpotted();
            OnReachingEnd();
        }
    }
}
