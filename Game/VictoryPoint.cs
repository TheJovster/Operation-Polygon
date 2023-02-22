using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Core 
{
    public class VictoryPoint : MonoBehaviour
    {
        private GameManager gameManager;
        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && gameManager.GetCurrentTime() > 0f)
            {
                SetVictoryCondition();
            }
        }

        private void SetVictoryCondition()
        {
            Debug.Log("Yay, you have won!");
            gameManager.LoadMainMenu();
        }
    }
}
