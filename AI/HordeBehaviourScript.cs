using UnityEngine;

namespace OperationPolygon.AI.Control 
{
    public class HordeBehaviourScript : MonoBehaviour
    {

        private bool isAlerted = false;
        void Update()
        {
            if(isAlerted) 
            {
                foreach (EnemyDetector detector in GetComponentsInChildren<EnemyDetector>())
                {
                    detector.SetAggro();
                }
            }

        }

        public void HordeAlert() 
        {
            isAlerted = true;
        }
    }
}
