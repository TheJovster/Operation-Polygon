using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    //camera
    [SerializeField] private CinemachineVirtualCamera aimCamera;

    //components
    private StarterAssetsInputs inputs;
    private ThirdPersonController controller;

    //variables
    [SerializeField] private float defaultMouseSensitivity = 1f;
    [SerializeField] private float mouseAimSensitivity = .333f;
    [SerializeField] private float aimMoveSpeed = 1f;
    [SerializeField] private float aimSprintSpeed = 2.65f;

    void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        AimState();
    }

    private void AimState() 
    {
        if (inputs.aim)
        {
            aimCamera.gameObject.SetActive(true);
            controller.SetMouseSensitivityFraction(mouseAimSensitivity);
            controller.SetMoveSpeed(aimMoveSpeed);
            controller.SetSprintSpeed(aimSprintSpeed);

        }
        else
        {
            aimCamera.gameObject.SetActive(false);
            controller.SetMouseSensitivityFraction(defaultMouseSensitivity);
            controller.ResetMoveSpeedValue();
            controller.ResetSprintSpeedValue();
        }
    }
}
