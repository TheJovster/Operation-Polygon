using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform aimPosVisual;
    private Inputs input;

    private void Awake()
    {
        mainCam = Camera.main;
        input = GetComponent<Inputs>();
    }

    private void Update()
    {
        if(Time.timeScale == 1)
        aimPosVisual.position = GetMouseWorldPosition();
    }

    public Vector3 GetMouseWorldPosition() 
    {
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else return Vector3.zero; 
        //need to be wary of this one, if there's no collision the system will break completely.
    }
}
