using OperationPolygon.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;

namespace OperationPolygon.Core 
{
    public class GameManager : MonoBehaviour
    {
        private float timeRemaining = 300f;

        [Tooltip("Time limit in seconds.")]
        [SerializeField] private float endGameTime = 0f;

        private bool gameIsOn = false;
        private bool gameOver = false;

        private GameObject player;
        private GameObject soundtrackManager;
        private UIManager uiManager;
        [SerializeField]private bool isPaused = false;

        [SerializeField] private AudioSource soundtrackSource;
        [SerializeField] private AudioSource menuSource;
        [SerializeField] private AudioSource gameOverSource;
        [SerializeField] private AudioSource victorySource;


        //private string mainMenuScene = "";


        void Awake()
        {
            gameIsOn = true;
            player = GameObject.FindGameObjectWithTag("Player");
            uiManager = player.GetComponent<UIManager>();
            soundtrackManager = GameObject.FindGameObjectWithTag("SoundtrackManager");
            soundtrackSource = soundtrackManager.GetComponent<AudioSource>();
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
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


            if (Input.GetKeyDown(KeyCode.Escape) && gameIsOn) 
            {
                if (isPaused) 
                {
                    Unpause();
                }
                else if (!isPaused) 
                {
                    Pause();
                }
            }
        }

        public void Pause() 
        {
            Time.timeScale = 0f;
            uiManager.ShowPauseMenu();
            soundtrackSource.Pause();
            menuSource.Play();
            isPaused = true;
        }

        public void Unpause() 
        {
            Time.timeScale = 1f;
            uiManager.HidePauseMenu();
            menuSource.Pause();
            soundtrackSource.Play();
            isPaused = false;
        }

        public void ReloadLevel() 
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadSceneAsync(currentSceneName);
        }


        private void TimerTick(float deltaTime) 
        {
            timeRemaining -= deltaTime;
            if (timeRemaining <= endGameTime)
            {
                TriggerGameOver();
            }
        }

        public bool IsGameOver() 
        {
            return gameOver;
        }

        public void TriggerGameOver() 
        {
            float lastTimeLookup = timeRemaining;
            timeRemaining = lastTimeLookup;
            gameIsOn = false;
            StartCoroutine(GameOverCondition());
        }

        private IEnumerator GameOverCondition() 
        {
            //game over logic
            soundtrackSource.Stop();
            menuSource.Stop();
            gameOverSource.Play();
            yield return new WaitForSeconds(3f);
            Time.timeScale = 0f;
            uiManager.ShowGameOverScreen();
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void TriggerVictory() 
        {
            float lastTimeLookup = timeRemaining;
            timeRemaining = lastTimeLookup;
            gameIsOn = false;
            Time.timeScale = 0;
            soundtrackSource.Stop();
            menuSource.Stop();
            gameOverSource.Stop();
            victorySource.Play();
            uiManager.ShowVictoryScreen();
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(0);
        }

        //event triggers



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

        //setters

        public void SetGameOver() 
        {
            gameOver = true;
            TriggerGameOver();
        }
    }

}