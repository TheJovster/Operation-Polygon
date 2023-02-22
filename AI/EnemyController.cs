using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationPolygon.AI.Control 
{
    public class EnemyController : MonoBehaviour
    {
        //components
        [SerializeField] private NavMeshAgent navMesh;
        [SerializeField] private Health health;
        private EnemyDetector detector;
        private Animator animator;

        //target
        Transform target;

        //private variables
        private string animSpeedID = "Speed";
        private int animSpeedHash;
        //serialized variables
        [SerializeField] private float attackRange;
        [SerializeField] private int attackDamage = 10;
        //public variables

        void Awake()
        {
            navMesh = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            detector = GetComponentInChildren<EnemyDetector>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            animSpeedHash = Animator.StringToHash(animSpeedID);
        }

        void Update()
        {
            HandleAnimation();
            if (detector.GetAggro() && health.IsAlive())
            {
                LookAtTarget();
                MoveToTarget();
                //move to target
            }
            else return;
        }

        private Vector3 GetLookAtPosition()
        {
            Vector3 lookAtTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
            return lookAtTarget;
        }

        private void LookAtTarget()
        {
            transform.LookAt(GetLookAtPosition(), Vector3.up);
        }

        private void MoveToTarget() 
        {
            if(DistanceToTarget() > attackRange) 
            {
                navMesh.isStopped = false;
                navMesh.SetDestination(target.position);
            }
            if(DistanceToTarget() <= attackRange) 
            {
                AttackBehaviour();
            }
        }

        private float DistanceToTarget() 
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            return distanceToTarget;
        }

        private void AttackBehaviour() 
        {
            if (target.GetComponent<Health>().IsAlive()) 
            {
                animator.SetTrigger("Attack");
            }
            //else { feeding logic }
        }

        public void SetAggro() 
        {
            detector.SetAggro();
        }

        //animation handling and events
        private void HandleAnimation() 
        {
            animator.SetFloat(animSpeedHash, navMesh.velocity.magnitude);
        }

        public void OnAttack() 
        {
            if(DistanceToTarget() <= attackRange) 
            {
                target.GetComponent<Health>().TakeDamage(attackDamage);
            }
        }
    }
}
