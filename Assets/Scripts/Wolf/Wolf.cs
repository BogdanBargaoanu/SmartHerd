using Assets.Scripts.AI.BehaviourTree;
using Assets.Scripts.AI.FSM;
using Assets.Scripts.AI.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Wolf
{
    public class Wolf : MonoBehaviour
    {
        [Header("Pack Behaviour")]
        public float wolfAvoidanceRadius = 2f;
        public float separationWeight = 3f;
        public LayerMask wolfLayer;

        [Header("World")]
        public float worldRadius = 50f;

        [Header("Movement")]
        public float speed = 6.5f;
        public float rotationSpeed = 5f;

        [Header("Vision")]
        public float visionRadius = 15f;
        public float eatDistance = 1.5f;

        [Header("Layers")]
        public LayerMask sheepLayer;

        [Header("Retreat")]
        public float safeZoneRadius = 40f;

        private Vector3 velocity;
        private Vector3 wanderDirection;

        private Transform currentTarget;

        private Node behaviorTree;

        private WolfUtilityAI utilityAI;

        private float wanderTimer;

        public WolfState currentState;

        void Start()
        {
            utilityAI = GetComponent<WolfUtilityAI>();

            behaviorTree = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CanSeeSheepNode(this),
                    new ChaseNode(this)
                }),

                new WanderNode(this)
            });

            PickRandomDirection();
        }

        void Update()
        {
            UpdateNeeds();

            currentState = utilityAI.DecideState();

            switch (currentState)
            {
                case WolfState.Hunt:
                    behaviorTree.Execute();
                    break;

                case WolfState.Retreat:
                    Retreat();
                    break;

                case WolfState.Idle:
                    Wander();
                    break;
            }

            Move();
        }

        void UpdateNeeds()
        {
            utilityAI.hunger += 5f * Time.deltaTime;

            utilityAI.stamina -= 2f * Time.deltaTime;

            utilityAI.stamina = Mathf.Clamp(
                utilityAI.stamina,
                0,
                100
            );

            utilityAI.hunger = Mathf.Clamp(
                utilityAI.hunger,
                0,
                100
            );
        }

        public Transform FindNearestSheep()
        {
            Collider[] colliders =
                Physics.OverlapSphere(
                    transform.position,
                    visionRadius,
                    sheepLayer
                );

            float minDistance = Mathf.Infinity;

            Transform nearest = null;

            foreach (Collider col in colliders)
            {
                if (col == null)
                    continue;

                float dist =
                    Vector3.Distance(
                        transform.position,
                        col.transform.position
                    );

                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = col.transform;
                }
            }

            currentTarget = nearest;

            return currentTarget;
        }

        public void ChaseTarget()
        {
            if (currentTarget == null)
            {
                FindNearestSheep();

                if (currentTarget == null)
                {
                    Wander();
                    return;
                }
            }

            Vector3 dir =
                (currentTarget.position - transform.position)
                .normalized;

            velocity = dir * speed;

            velocity += CalculateWolfSeparation();

            float dist =
                Vector3.Distance(
                    transform.position,
                    currentTarget.position
                );

            if (dist < eatDistance)
            {
                GameObject sheepToDestroy =
                    currentTarget.gameObject;

                currentTarget = null;

                Destroy(sheepToDestroy);

                utilityAI.hunger =
                    Mathf.Max(
                        0,
                        utilityAI.hunger - 40f
                    );

                utilityAI.stamina =
                    Mathf.Min(
                        100,
                        utilityAI.stamina + 20f
                    );
            }
        }

        public void Wander()
        {
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= 2f)
            {
                PickRandomDirection();
                wanderTimer = 0f;
            }

            velocity =
                wanderDirection *
                speed *
                0.5f;

            velocity += CalculateWolfSeparation();
        }

        void PickRandomDirection()
        {
            wanderDirection =
                new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f)
                ).normalized;
        }

        void Retreat()
        {
            Vector3 dir =
                (-transform.position)
                .normalized;

            velocity = dir * speed;

            velocity += CalculateWolfSeparation();
        }

        void Move()
        {
            velocity.y = 0;

            if (transform.position.magnitude > worldRadius)
            {
                Vector3 centerForce =
                    (-transform.position).normalized;

                velocity += centerForce * speed;
            }

            if (velocity.sqrMagnitude > 0.01f)
            {
                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(
                            velocity.normalized
                        ),
                        rotationSpeed * Time.deltaTime
                    );
            }

            transform.position +=
                velocity * Time.deltaTime;
        }

        private Vector3 CalculateWolfSeparation()
        {
            Vector3 separation = Vector3.zero;

            Collider[] wolves =
                Physics.OverlapSphere(
                    transform.position,
                    wolfAvoidanceRadius,
                    wolfLayer
                );

            foreach (Collider wolf in wolves)
            {
                if (wolf.gameObject == gameObject)
                    continue;

                float dist =
                    Vector3.Distance(
                        transform.position,
                        wolf.transform.position
                    );

                if (dist > 0.01f)
                {
                    separation +=
                        (transform.position -
                         wolf.transform.position).normalized
                        / dist;
                }
            }

            return separation.normalized * separationWeight;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                transform.position,
                visionRadius
            );
        }
    }
}