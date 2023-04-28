using Unity.VisualScripting;
using UnityEngine;

namespace OperationPolygon.Combat 
{

    public class AmmoInventory : MonoBehaviour
    {
        private int currentAmmoInUse;
        private int currentAssaultRifleAmmo;
        private int currentSMGAmmo;
        private int currentLMGAmmo;
        private int currentGrenades;
        private int currentSniperAmmo;
        [SerializeField] private int startingAssaultRifleAmmo;
        [SerializeField] private int startingSMGAmmo;
        [SerializeField] private int startingLMGAmmo;
        [SerializeField] private int startingGrenades;
        [SerializeField] private int startingSniperAmmo;
        [SerializeField] private int maxAssaultRifleAmmoInInventory;
        [SerializeField] private int maxSMGAmmoInInventory;
        [SerializeField] private int maxLMGAmmoInInventory;
        [SerializeField] private int maxGrenadesInInventory;
        [SerializeField] private int maxSniperAmmo;
        private Weapon currentWeapon;
        private WeaponInventory weaponInventory;

        private void Awake()
        {
            
            weaponInventory = GetComponent<WeaponInventory>();
            currentWeapon = weaponInventory.CurrentWeapon;
        }

        private void Start()
        {
            InitializeCurrentWeaponData();
        }

        public void InitializeCurrentWeaponData()
        {
            currentWeapon = weaponInventory.CurrentWeapon;
            switch (currentWeapon.WeaponClass)
            {
                case WeaponClass.AssaultRifle:
                    currentAmmoInUse = currentAssaultRifleAmmo;
                    break;

                case WeaponClass.SMG:
                    currentAmmoInUse = currentSMGAmmo;
                    break;
                case WeaponClass.LMG:
                    currentAmmoInUse = currentLMGAmmo;
                    break;
                case WeaponClass.Launcher:
                    currentAmmoInUse = currentGrenades;
                    break;
                case WeaponClass.SniperRifle:
                    currentAmmoInUse = currentSniperAmmo;
                    break;

            }
        }

        public void SaveCurrentAmmo()
        {
            currentWeapon = weaponInventory.CurrentWeapon;
            switch (currentWeapon.WeaponClass)
            {
                case WeaponClass.AssaultRifle:
                    currentAssaultRifleAmmo = currentAmmoInUse;
                    break;
                case WeaponClass.SMG:
                    currentSMGAmmo = currentAmmoInUse;
                    break;
                case WeaponClass.LMG:
                    currentLMGAmmo = currentAmmoInUse; ;
                    break;
                case WeaponClass.Launcher:
                    currentGrenades = currentAmmoInUse;
                    break;
                case WeaponClass.SniperRifle:
                    currentAmmoInUse = currentSniperAmmo;
                    break;
            }
        }

        public void AddAmmo(int ammoToAdd) 
        {
            switch (currentWeapon.WeaponClass)
            {
                case WeaponClass.AssaultRifle:
                    currentAssaultRifleAmmo += ammoToAdd;
                    if(currentAssaultRifleAmmo >= maxAssaultRifleAmmoInInventory) 
                    {
                        currentAssaultRifleAmmo = maxAssaultRifleAmmoInInventory;
                    }
                    currentAmmoInUse = currentAssaultRifleAmmo;
                    break;

                case WeaponClass.SMG:
                    currentSMGAmmo += ammoToAdd;
                    if (currentSMGAmmo >= maxAssaultRifleAmmoInInventory)
                    {
                        currentAssaultRifleAmmo = maxSMGAmmoInInventory;
                    }
                    currentAmmoInUse = currentSMGAmmo;
                    break;
                case WeaponClass.LMG:
                    currentLMGAmmo += ammoToAdd;
                    if(currentLMGAmmo >= maxLMGAmmoInInventory)
                    {
                        currentLMGAmmo = maxLMGAmmoInInventory;
                    }
                    currentAmmoInUse = currentLMGAmmo;
                    break;
                case WeaponClass.Launcher:
                    currentGrenades += ammoToAdd;
                    if(currentGrenades >= maxGrenadesInInventory) 
                    {
                        currentGrenades = maxGrenadesInInventory;
                    }
                    currentAmmoInUse = currentGrenades;
                    break;
                case WeaponClass.SniperRifle:
                    currentSniperAmmo += ammoToAdd;
                    if(currentSniperAmmo >= maxSniperAmmo) 
                    {
                        currentSniperAmmo = maxSniperAmmo;
                    }
                    currentAmmoInUse = currentSniperAmmo;
                    break;
            }
        }

        public void RemoveAmmo(int ammoToRemove)
        { 
            currentAmmoInUse -= ammoToRemove;
            if(currentAmmoInUse < 0) 
            {
                currentAmmoInUse = 0;
            }
        }


        public int GetCurrentAmmoInInventory() 
        {
            return currentAmmoInUse;
        }

    }
}
