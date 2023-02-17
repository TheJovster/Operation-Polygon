using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationPolygon.Combat;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]private Weapon currentWeapon; //probably not the best practice, but this is done for testing purposes and will be refactored later.
    [SerializeField] private TextMeshProUGUI ammoCount; //refactor names and everything later.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ammoCount.text = currentWeapon.GetCurrentAmmoInMag().ToString();
    }
}
