using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OperationPolygon.Core 
{
    public class GameManager : MonoBehaviour
    {
        private float timeRemaining = 300f;
        [Tooltip("Time limit in seconds.")]
        [SerializeField] private float endGameTime = 0f;

        private bool gameIsOn = false;
        private bool gameOver = false;

        private string mainMenuScene = "";


        void Start()
        {
            gameIsOn = true;
        }

        void Update()
        {
            if(gameIsOn && !gameOver) 
            {
                TimerTick(Time.deltaTime);
            }
            if(timeRemaining <= endGameTime) 
            {
                TriggerGameOver();
            }
        }

        private void TimerTick(float deltaTime) 
        {
            timeRemaining -= deltaTime;
        }

        public bool IsGameOver() 
        {
            return gameOver;
        }

        public void TriggerGameOver() 
        {
            timeRemaining = 0f;
            gameOver = true;
            gameIsOn = false;
            GameOverCondition();
        }

        private void GameOverCondition() 
        {
            //game over logic
            Time.timeScale = 0f;
        }



        //event triggers

        public void ReturnToMainMenu() 
        {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync("");
        }

        //getters

        public float GetCurrentTime() 
        {
            return timeRemaining;
        }

        public void LoadMainMenu() 
        {

        }
    }

}