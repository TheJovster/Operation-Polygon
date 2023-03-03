using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weaponsList; //I'm using a List because it's easier to expand at runtime than Arrays.
        private Weapon equippedWeapon;

        private void Awake()
        {
            equippedWeapon = GetComponentInChildren<Weapon>();

        }

        private void Start()
        {
            SetDefaultWeapon();
        }

        public void SetDefaultWeapon()  //done at awake when the game starts
        {
            
            weaponsList.Add(equippedWeapon);
        }

        public Weapon GetEquippedWeapon() 
        {
            return equippedWeapon;
        }

        public List<Weapon> GetWeaponsList() 
        {
            return weaponsList;
        }
    }
}
