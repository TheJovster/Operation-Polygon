using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class AmmoInventory : MonoBehaviour
    {
        [SerializeField] private int currentAmmoInInventory;
        [SerializeField] private int startingAmmo;
        [SerializeField] private int startingGrenades;
        [SerializeField] private int maxAmmoInInventory;
        [SerializeField] private int maxGrenadesInInventory;
        private Weapon currentWeapon;

        private void Start()
        {
            SetupCurrentWeaponData();

        }

        public void SetupCurrentWeaponData()
        {
            currentWeapon = GetComponentInChildren<Weapon>();
            if (!currentWeapon.IsLauncher)
                currentAmmoInInventory = startingAmmo;
            else if (currentWeapon.IsLauncher)
            {
                currentAmmoInInventory = startingGrenades;
            }
        }

        public void AddAmmo(int ammoToAdd) 
        {
            currentAmmoInInventory += ammoToAdd;
            if(currentAmmoInInventory >= maxAmmoInInventory) 
            {
                currentAmmoInInventory = maxAmmoInInventory;
            }
        }

        public void AddGrenades(int ammoToAdd) 
        {
            currentAmmoInInventory += ammoToAdd;
            if(currentAmmoInInventory >= maxGrenadesInInventory) 
            {
                currentAmmoInInventory = maxGrenadesInInventory;
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
