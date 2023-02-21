using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private int currentStamina;
    [SerializeField] private int maxStamina = 100;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        ReplenishStamina();
    }

    public void DrainStamina() 
    {

    }

    private void ReplenishStamina() 
    {

    }

    public float GetStaminaPercentage() 
    {
        float percentage = currentStamina / maxStamina;
        return percentage;
    }
}
