using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpotted;

    public float speed = 5f;
    public float waitTime = 0.5f;
    public float turnSpeed = 90f;
    public float timeForEscape = 1f;

    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    public LayerMask spottedMask;

    public Transform pathHolder;
    Transform player;
    Color originalSpotlightColor;

    float timeSinceSpotted;



    void Start()
    {
        //field of view
        originalSpotlightColor = spotlight.color;
        viewAngle = spotlight.spotAngle;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        //waypoints list creating
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }

        //Guard start position
        Vector3 startPosition = pathHolder.GetChild(0).position;
        transform.position = startPosition;
        transform.LookAt(pathHolder.GetChild(1).position);

        //Guard Patrol
        StartCoroutine(GuardWalk(waypoints));
    }

    private void Update()
    {   
        //Spotting player
        if (CanSeePlayer())
        {
            timeSinceSpotted += Time.deltaTime;
        }
        else
        {
            spotlight.color = originalSpotlightColor;
            timeSinceSpotted -= Time.deltaTime;
        }
        timeSinceSpotted = Mathf.Clamp(timeSinceSpotted, 0, timeForEscape);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, timeSinceSpotted / timeForEscape);
        if (timeSinceSpotted >= timeForEscape)
        {
            if (OnGuardSpotted != null)
            {
                OnGuardSpotted ();
                print("Spotted!");
            }
        }
    }



    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, spottedMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    IEnumerator GuardWalk(Vector3[] waypoints)
    {
        int nextWaypointIndex = 1;
        Vector3 nextWayPoint = waypoints[nextWaypointIndex];
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextWayPoint, speed * Time.deltaTime);

            if (transform.position == nextWayPoint)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % waypoints.Length;
                nextWayPoint = waypoints[nextWaypointIndex];
                yield return StartCoroutine(TurnToNextWaypoint(nextWayPoint));
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }
    IEnumerator TurnToNextWaypoint(Vector3 lookTarget)
    {
        Vector3 directionToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z, directionToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
    void OnDrawGizmos()
    {
        Vector3 waypointStartPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = waypointStartPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.2f);
            Gizmos.DrawLine(previousPosition, waypoint.position);

            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, waypointStartPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
