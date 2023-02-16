using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform aimPosVisual;
    private StarterAssetsInputs input;

    private void Awake()
    {
        mainCam = Camera.main;
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        Ray ray =  mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray, out RaycastHit hit)) 
        {
            aimPosVisual.position = hit.point;
        }
    }
}
