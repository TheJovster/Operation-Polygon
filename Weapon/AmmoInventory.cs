using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class AmmoInventory : MonoBehaviour
    {
        [SerializeField] private int currentAmmoInInventory;
        [SerializeField] private int startingAmmo;
        [SerializeField] private int maxAmmoInInventory;

        private void Start()
        {
            currentAmmoInInventory = startingAmmo;
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
            if(currentAmmoInInventory < 0) 
            {
                currentAmmoInInventory = 0;
            }
        }


        public int GetCurrentAmmoInInventory() 
        {
            return currentAmmoInInventory;
        }
    }
}
