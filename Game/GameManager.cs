using OperationPolygon.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.Diagnostics;

namespace OperationPolygon.Core 
{
    public class GameManager : MonoBehaviour
    {
        private float timeRemaining = 300f;

        [Tooltip("Time limit in seconds.")]
        [SerializeField] private float endGameTime = 0f;

        private bool gameIsOn = false;
        private bool gameOver = false;

        [SerializeField] private GameObject pauseMenu;
        [SerializeField]private bool isPaused = false;

        //private string mainMenuScene = "";


        void Awake()
        {
            gameIsOn = true;
        }

        private void Start()
        {
            
        }

        void Update()
        {
            if (gameIsOn && !gameOver)
            {
                TimerTick(Time.deltaTime);
            }
            if (timeRemaining <= endGameTime)
            {
                TriggerGameOver();
            }

            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                ToggleMainMenu();
            }
        }

        private void ToggleMainMenu() 
        {
            Debug.Log("ToggleMainMenu");
            isPaused = !isPaused;
            if (isPaused) 
            {
                Time.timeScale = 1;
                isPaused = false;
            }
            else if (!isPaused) 
            {
                Time.timeScale = 0;
                isPaused = true;
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

        public bool IsPaused() 
        {
            return isPaused;
        }
    }

}