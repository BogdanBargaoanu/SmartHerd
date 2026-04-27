using UnityEngine;

public class Wolf : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6.5f; // Slightly faster than sheep
    public float rotationSpeed = 5f;

    [Header("Predator Settings")]
    public float visionRadius = 15f;
    public float eatDistance = 1.5f;
    public LayerMask sheepLayer;

    private Vector3 velocity;
    private Vector3 wanderTarget;

    void Start()
    {
        velocity = transform.forward * speed;
        InvokeRepeating(nameof(PickNewWanderTarget), 0f, 3f);
    }

    void PickNewWanderTarget()
    {
        // Pick a random direction on the XZ plane
        wanderTarget = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        Vector3 targetDirection = wanderTarget;
        Transform nearestSheep = null;
        float minDistance = float.MaxValue;

        // Look for prey
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionRadius, sheepLayer);

        foreach (Collider col in colliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestSheep = col.transform;
            }
        }

        // Chase logic
        if (nearestSheep != null)
        {
            targetDirection = (nearestSheep.position - transform.position).normalized;

            if (minDistance < eatDistance)
            {
                // Consume the sheep
                Destroy(nearestSheep.gameObject);
            }
        }
        else
        {
            // Boundary logic for wandering wolves
            if (transform.position.magnitude > 40f)
            {
                targetDirection = (Vector3.zero - transform.position).normalized;
            }
        }

        // Apply Forces
        Vector3 desiredVelocity = targetDirection * speed;
        Vector3 acceleration = (desiredVelocity - velocity);

        velocity += acceleration * Time.deltaTime;
        velocity.y = 0; // Lock to ground plane
        velocity = Vector3.ClampMagnitude(velocity, speed);

        // Move and Rotate
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), rotationSpeed * Time.deltaTime);
        }

        transform.position += velocity * Time.deltaTime;
    }
}