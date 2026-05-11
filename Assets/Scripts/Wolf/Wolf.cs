using Assets.Scripts.AI.BehaviourTree;
using Assets.Scripts.AI.FSM;
using Assets.Scripts.AI.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Wolf
{
    public class Wolf : MonoBehaviour
    {
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
            Collider[] colliders = Physics.OverlapSphere(
                transform.position,
                visionRadius,
                sheepLayer
            );

            float minDistance = Mathf.Infinity;

            currentTarget = null;

            foreach (Collider col in colliders)
            {
                float dist = Vector3.Distance(
                    transform.position,
                    col.transform.position
                );

                if (dist < minDistance)
                {
                    minDistance = dist;
                    currentTarget = col.transform;
                }
            }

            return currentTarget;
        }

        public void ChaseTarget()
        {
            if (currentTarget == null)
                return;

            Vector3 dir =
                (currentTarget.position - transform.position)
                .normalized;

            velocity = dir * speed;

            float dist = Vector3.Distance(
                transform.position,
                currentTarget.position
            );

            if (dist < eatDistance)
            {
                Destroy(currentTarget.gameObject);

                utilityAI.hunger -= 40f;

                utilityAI.stamina += 20f;
            }
        }

        public void Wander()
        {
            if (Random.value < 0.01f)
            {
                PickRandomDirection();
            }

            velocity = wanderDirection * speed * 0.5f;
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
                (Vector3.zero - transform.position)
                .normalized;

            velocity = dir * speed;
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
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                transform.position,
                visionRadius
            );
        }
    }
}