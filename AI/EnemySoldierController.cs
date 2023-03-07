using OperationPolygon.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace OperationPolygon.AI 
{
    public class EnemySoldierController : MonoBehaviour
    {
        [SerializeField] private Vector3 aimOffset;

        private Transform playerTransform;
        private EnemyDetector detector;
        //private AIWeaponIK weaponIK;

        private List<Transform> patrolPaths = new List<Transform>(); //basis for patrol paths
                                                                     //float for error margin when finding the next patrol path.

        private void Awake()
        {
            //weaponIK = GetComponent<AIWeaponIK>();
        }
        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            //weaponIK.SetTargetTransform(playerTransform);
        }

        private void Update()
        {
            LookAtPlayer(); 

        }

        private void LookAtPlayer()
        {
            Vector3 playerPositionAim = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(playerPositionAim);
        }

        //SearchBehaviour

        //ChaseBehaviour

        //EngageBehaviour

        //RetreatBehaviour

        //charge behaviour

        //PatrolBehaviour

        //Seek reinforcements behaviour

        //???
    }
}