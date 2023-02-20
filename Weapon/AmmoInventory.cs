using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class AmmoInventory : MonoBehaviour
    {
        [SerializeField] private int currentAmmoInInventory;
        [SerializeField] private int maxAmmoInInventory;

        private void Start()
        {
            currentAmmoInInventory = maxAmmoInInventory;
        }

        public void AddAmmo(int ammoToAdd) 
        {
            currentAmmoInInventory += ammoToAdd;
            if(currentAmmoInInventory >= maxAmmoInInventory) 
            {
                currentAmmoInInventory = maxAmmoInInventory;
            }
        }

        public void RemoveAmmo(int ammoToRemove) 
        {
            currentAmmoInInventory -= ammoToRemove;
        }


        public int GetCurrentAmmInInventory() 
        {
            return currentAmmoInInventory;
        }
    }
}
