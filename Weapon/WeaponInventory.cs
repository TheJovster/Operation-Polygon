using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class WeaponInventory : MonoBehaviour
    {
        public static WeaponInventory Instance { get; private set; }

        [SerializeField] private Weapon[] weaponsList; //make sure that your weapoinsList array's length matches the number of cases in weapon Switching.
        //The object that contains the weapon game objects. In this case, it's Hand_R game object in the PlayerArmature child object named Root.
        [field: SerializeField] public Transform WeaponContainer { get; private set; }
        public Weapon CurrentWeapon { get; private set; }
        private AmmoInventory ammoInventory;

        int weaponIndex = 0;

        private void Awake()
        {

            ammoInventory = GetComponent<AmmoInventory>();
        }

        private void Start()
        {
            Instance = this;
        }


        public void SetCurrentWeapon(Weapon weapon) 
        {
            CurrentWeapon = weapon;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && WeaponContainer.childCount > 1)
            {
                DecrementWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && WeaponContainer.childCount > 1)
            {
                IncrementWeapon();
            }

        }

        private void IncrementWeapon() //adds +1 to weapon index and switches the current weapon
        {
            weaponIndex++;
            if(weaponIndex > weaponsList.Length - 1) 
            {
                weaponIndex = 0;
            }
            ammoInventory.SaveCurrentAmmo();
            foreach (Transform child in WeaponContainer)
            {
                child.gameObject.SetActive(false);
            }
            weaponsList[weaponIndex].gameObject.SetActive(true);
            UIManager.Instance.GetWeaponName();
        }

        private void DecrementWeapon() //substracts +1 from the weapon index and switches to the desired weapon
        {
            weaponIndex--;
            if (weaponIndex < 0)
            {
                weaponIndex = weaponsList.Length - 1;
            }
            ammoInventory.SaveCurrentAmmo();
            foreach (Transform child in WeaponContainer)
            {
                child.gameObject.SetActive(false);
            }
            weaponsList[weaponIndex].gameObject.SetActive(true);
            UIManager.Instance.GetWeaponName();
        }
    }
}
