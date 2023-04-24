using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    [field: SerializeField] public int NumberOfHealthPacks { get; private set; }
    private int maxNumofHealthPacks = 10;

    [SerializeField] private ParticleSystem healFX;

    private Health health;
    private void Awake()
    {
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        HealPlayer();

    }

    private void HealPlayer()
    {
        if (Input.GetKeyDown(KeyCode.H) && NumberOfHealthPacks > 0)
        {
            if (NumberOfHealthPacks <= 0)
            {
                Debug.Log("Can't heal. No healthpacks present");
                return;
            }
            else if(health.GetCurrentHealth() < health.GetMaxHealth()) 
            {
                UseHealthPack(20); 
            }
            else 
            {
                return;
            }


        }
    }

    public void AddHealthPack(int numberToAdd) 
    {
        if(NumberOfHealthPacks < maxNumofHealthPacks) 
        {
            NumberOfHealthPacks += numberToAdd;
        }
        else if(NumberOfHealthPacks == maxNumofHealthPacks) 
        {
            NumberOfHealthPacks = maxNumofHealthPacks;
        }
    }

    public void UseHealthPack(int healthToAdd) 
    {
        //health pack item class will be implemnted later - right now, I'll just have a placeholder value
        health.AddHealth(healthToAdd);
        NumberOfHealthPacks--;
        Debug.Log("Player healed");
        Debug.Log("Player has " + NumberOfHealthPacks + " Health Packs left.");
        healFX.Play();
        //I don't have access to heal animations, so I'll use a particle system to show effect of a healthpack being used instead
    }
}
