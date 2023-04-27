using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private Weapon[] weaponsList; //I'm using a List because it's easier to expand at runtime than Arrays.
        [Header("The object that contains the weapon game objects. In this case, it's Hand_R game object in the PlayerArmature child object named Root.")]
        [SerializeField] private Transform weaponContainer;
        public Weapon CurrentWeapon { get; private set; }

        public void SetCurrentWeapon(Weapon weapon) 
        {
            CurrentWeapon = weapon;
            Debug.Log("Current weapon is " + CurrentWeapon.WeaponName);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                foreach(Transform child in weaponContainer) 
                {
                    child.gameObject.SetActive(false);
                }
                weaponsList[0].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                foreach (Transform child in weaponContainer)
                {
                    child.gameObject.SetActive(false);
                }
                weaponsList[1].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                foreach (Transform child in weaponContainer)
                {
                    child.gameObject.SetActive(false);
                }
                weaponsList[2].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) 
            {
                foreach (Transform child in weaponContainer)
                {
                    child.gameObject.SetActive(false);
                }
                weaponsList[3].gameObject.SetActive(true);
            }
        }

    }
}
