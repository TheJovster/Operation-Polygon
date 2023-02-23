using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationPolygon.Core;

namespace OperationPolygon.AI 
{
    public class EnemyDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;

        private bool isAggroed = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player") 
            {
                isAggroed = true;
            }
        }

/*        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Player") 
            {
                isAggroed = false;
            }
//the zombies should just keep chasing the player
        }*/

        public bool GetAggro() 
        {
            return isAggroed;
        }

        public void SetAggro() 
        {
            isAggroed = true;
        }
    }
}
