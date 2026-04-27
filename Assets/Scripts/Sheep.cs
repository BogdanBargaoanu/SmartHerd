using UnityEngine;

public class Sheep : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 4f;

    [Header("Boid Radii")]
    public float neighborRadius = 5f;
    public float avoidanceRadius = 2f;
    public float predatorAvoidanceRadius = 10f;

    [Header("Boid Weights")]
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 1.5f;
    public float fleeWeight = 5f;

    [Header("Layers")]
    public LayerMask sheepLayer;
    public LayerMask wolfLayer;

    private Vector3 velocity;
    private Animator anim;
    private bool isDead = false;
    private string currentAnimState = "";

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        velocity = transform.forward * speed;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        ChangeAnimation("dead");

        velocity = Vector3.zero;
        this.enabled = false;
        Destroy(gameObject, 0.5f); // Quick destroy for prey
    }

    // Helper method to prevent calling CrossFade every single frame
    void ChangeAnimation(string newState)
    {
        if (currentAnimState == newState || anim == null) return;
        anim.CrossFade(newState, 0.2f);
        currentAnimState = newState;
    }

    void Update()
    {
        if (isDead) return;

        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 flee = Vector3.zero;

        int neighborCount = 0;
        float maxRadius = Mathf.Max(neighborRadius, predatorAvoidanceRadius);
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxRadius);

        foreach (Collider col in colliders)
        {
            if (col.gameObject == this.gameObject) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);

            if ((sheepLayer.value & (1 << col.gameObject.layer)) > 0 && dist < neighborRadius)
            {
                cohesion += col.transform.position;
                alignment += col.transform.forward;
                neighborCount++;

                if (dist < avoidanceRadius && dist > 0)
                    separation += (transform.position - col.transform.position) / dist;
            }

            if ((wolfLayer.value & (1 << col.gameObject.layer)) > 0 && dist < predatorAvoidanceRadius && dist > 0)
            {
                flee += (transform.position - col.transform.position) / dist;
            }
        }

        if (neighborCount > 0)
        {
            cohesion = (cohesion / neighborCount - transform.position).normalized * cohesionWeight;
            alignment = (alignment / neighborCount).normalized * alignmentWeight;
        }

        separation = separation.normalized * separationWeight;
        flee = flee.normalized * fleeWeight;

        Vector3 acceleration = cohesion + alignment + separation + flee;
        velocity += acceleration * Time.deltaTime;

        // Flat terrain lock
        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity, speed);

        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), rotationSpeed * Time.deltaTime);
        }
        transform.position += velocity * Time.deltaTime;

        if (velocity.magnitude > 0.1f)
        {
            ChangeAnimation("run_forward");
        }
    }
}