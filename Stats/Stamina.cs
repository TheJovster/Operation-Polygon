using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace OperationPolygon.Core 
{
    public class Stamina : MonoBehaviour
    {
        [SerializeField] private float currentStamina;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float walkDrainRate = 3f;
        [SerializeField] private float runDrainRate = 6f;
        [SerializeField] private float regenRate = 1f;
        [SerializeField]private bool isMoving = false;
        private Inputs input;
        private Health health;

        private void Awake()
        {
            currentStamina = maxStamina;
            input = GetComponent<Inputs>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            if (health.IsAlive()) 
            {
                RegenStaminaOverTime(Time.deltaTime);
                DrainStamina(Time.deltaTime);
            }
        }

        private void DrainStamina(float deltaTime)
        {
            if (isMoving) 
            {
                currentStamina -= walkDrainRate * deltaTime;
                if(currentStamina <= 0f) 
                {
                    currentStamina = 0f;
                    return;
                }
            }
            else if(isMoving && input.sprint) 
            {
                currentStamina -= runDrainRate * deltaTime;
                if (currentStamina <= 0f)
                {
                    currentStamina = 0f;
                    return;
                }
            }

        }

        public void DecreaseStamina(float amount) 
        {
            currentStamina -= amount;
        }

        public void RegenStaminaOverTime(float deltaTime)
        {
            if (!isMoving) 
            {
                currentStamina += regenRate * Time.deltaTime;
                if(currentStamina >= maxStamina) 
                {
                    currentStamina = maxStamina;
                    return;
                }
            }
        }

        public void RegenStamina(float amount) 
        {
            currentStamina += amount;
            if(currentStamina >= maxStamina) 
            {
                currentStamina = maxStamina;
            }
        }

        public float GetStaminaPercentage()
        {
            float percentage = currentStamina / maxStamina;
            return percentage;
        }

        public bool GetIsMoving() 
        {
            return isMoving;
        }

        public void SetIsMoving(bool value) 
        {
            isMoving = value;
        }

        public bool HasStamina() 
        {
            if (currentStamina > 0) return true;
            else return false;
        }
    }
}
