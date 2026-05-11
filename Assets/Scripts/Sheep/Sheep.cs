using Assets.Scripts.AI.FSM;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 4f;

    [Header("Boid Settings")]
    public float neighborRadius = 5f;
    public float avoidanceRadius = 2f;
    public float predatorAvoidanceRadius = 10f;

    [Header("Weights")]
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 1.5f;
    public float fleeWeight = 5f;

    [Header("Layers")]
    public LayerMask sheepLayer;
    public LayerMask wolfLayer;

    private Vector3 velocity;

    private SheepState currentState;

    private Transform nearbyWolf;

    void Start()
    {
        currentState = SheepState.Wander;

        velocity = transform.forward * speed;
    }

    void Update()
    {
        UpdateState();

        switch (currentState)
        {
            case SheepState.Wander:
                Wander();
                break;

            case SheepState.Flock:
                Flock();
                break;

            case SheepState.Flee:
                Flee();
                break;
        }

        Move();
    }

    void UpdateState()
    {
        nearbyWolf = FindNearbyWolf();

        if (nearbyWolf != null)
        {
            currentState = SheepState.Flee;
            return;
        }

        Collider[] sheep =
            Physics.OverlapSphere(
                transform.position,
                neighborRadius,
                sheepLayer
            );

        if (sheep.Length > 3)
        {
            currentState = SheepState.Flock;
        }
        else
        {
            currentState = SheepState.Wander;
        }
    }

    Transform FindNearbyWolf()
    {
        Collider[] wolves =
            Physics.OverlapSphere(
                transform.position,
                predatorAvoidanceRadius,
                wolfLayer
            );

        float minDist = Mathf.Infinity;

        Transform closest = null;

        foreach (Collider wolf in wolves)
        {
            float dist =
                Vector3.Distance(
                    transform.position,
                    wolf.transform.position
                );

            if (dist < minDist)
            {
                minDist = dist;
                closest = wolf.transform;
            }
        }

        return closest;
    }

    void Wander()
    {
        if (Random.value < 0.02f)
        {
            velocity =
                new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f)
                ).normalized * speed;
        }
    }

    void Flock()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;

        int count = 0;

        Collider[] sheep =
            Physics.OverlapSphere(
                transform.position,
                neighborRadius,
                sheepLayer
            );

        foreach (Collider col in sheep)
        {
            if (col.gameObject == gameObject)
                continue;

            float dist =
                Vector3.Distance(
                    transform.position,
                    col.transform.position
                );

            cohesion += col.transform.position;

            alignment += col.transform.forward;

            if (dist < avoidanceRadius)
            {
                separation +=
                    (transform.position - col.transform.position)
                    / dist;
            }

            count++;
        }

        if (count > 0)
        {
            cohesion =
                ((cohesion / count) - transform.position)
                .normalized * cohesionWeight;

            alignment =
                (alignment / count)
                .normalized * alignmentWeight;
        }

        separation =
            separation.normalized * separationWeight;

        Vector3 accel =
            cohesion + alignment + separation;

        velocity += accel * Time.deltaTime;

        velocity =
            Vector3.ClampMagnitude(
                velocity,
                speed
            );
    }

    void Flee()
    {
        if (nearbyWolf == null)
            return;

        Vector3 fleeDir =
            (transform.position - nearbyWolf.position)
            .normalized;

        velocity = fleeDir * speed * 1.5f;
    }

    void Move()
    {
        velocity.y = 0;

        if (velocity != Vector3.zero)
        {
            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(velocity),
                    rotationSpeed * Time.deltaTime
                );
        }

        transform.position += velocity * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            neighborRadius
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            predatorAvoidanceRadius
        );
    }
}