using Unity.VisualScripting;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class AmmoInventory : MonoBehaviour, IDataPersistence
    {
        public static AmmoInventory Instance { get; private set; }

        private int currentAmmoInUse;
        private int currentAssaultRifleAmmo;
        private int currentSMGAmmo;
        private int currentLMGAmmo;
        private int currentGrenades;
        private int currentSniperAmmo;
        [field: SerializeField] public int StartingAssaultRifleAmmo { get; private set; }
        [field: SerializeField] public int StartingSMGAmmo { get; private set; }
        [field: SerializeField] public int StartingLMGAmmo { get; private set; }
        [field: SerializeField] public int StartingGrenades { get; private set; }
        [field: SerializeField] public int StartingSniperRifleAmmo { get; private set; }
        [SerializeField] private int maxAssaultRifleAmmoInInventory;
        [SerializeField] private int maxSMGAmmoInInventory;
        [SerializeField] private int maxLMGAmmoInInventory;
        [SerializeField] private int maxGrenadesInInventory;
        [SerializeField] private int maxSniperRifleAmmo;
        private Weapon currentWeapon;
        private WeaponInventory weaponInventory;

        private void Awake()
        {
            Instance = this;
            weaponInventory = GetComponent<WeaponInventory>();
            currentWeapon = weaponInventory.CurrentWeapon;
        }

        private void Start()
        {

            InitializeCurrentWeaponData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I)) 
            {
                DataPersistenceManager.Instance.SaveGame();
            }

            if (Input.GetKeyDown(KeyCode.U)) 
            {
                DataPersistenceManager.Instance.LoadGame();
            }
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
                    if(currentSniperAmmo >= maxSniperRifleAmmo) 
                    {
                        currentSniperAmmo = maxSniperRifleAmmo;
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

        public void LoadData(GameData data)
        {
            this.currentAssaultRifleAmmo = data.currentAssaultRifleAmmo;
            this.currentSMGAmmo = data.currentSMGAmmo;
            this.currentLMGAmmo = data.currentLMGAmmo;
            this.currentSniperAmmo = data.currentSniperRifleAmmo;
            this.currentGrenades = data.currentGrenades;
        }

        public void SaveData(ref GameData data)
        {
            data.currentAssaultRifleAmmo = this.currentAssaultRifleAmmo;
            data.currentSMGAmmo = this.currentSMGAmmo;
            data.currentLMGAmmo = this.currentLMGAmmo;
            data.currentSniperRifleAmmo = this.currentSniperAmmo;
            data.currentGrenades = this.currentGrenades;
        }
    }
}
