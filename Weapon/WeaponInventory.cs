using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weaponsList; //I'm using a List because it's easier to expand at runtime than Arrays.
        private Weapon equippedWeapon;

        public void SetDefaultWeapon()  //done at awake when the game starts
        {
        
        }
    }
}
