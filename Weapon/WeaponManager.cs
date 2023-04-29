using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat
{
    public class WeaponManager : MonoBehaviour
    {
        public static WeaponManager Instance { get; private set; }

        [SerializeField] private GameObject[] weapons;

        //bools to check which weapons the player has

        [field: SerializeField] public bool HasM4 { get; private set; }
        [field: SerializeField] public bool HasAK12 { get; private set; }
        [field: SerializeField] public bool HasMPX { get; private set; }
        [field: SerializeField] public bool HasG28 { get; private set; }
        [field: SerializeField] public bool Has249 { get; private set; }
        [field: SerializeField] public bool HasMGL { get; private set; }


        private void Awake()
        {
            Instance = this;
            HasM4 = true;

        }

    }
}
