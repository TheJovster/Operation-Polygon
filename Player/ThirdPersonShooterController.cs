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

    [SerializeField] private float defaultMoveSpeedFraction = 1f;
    [SerializeField] private float aimMoveSpeedFraction = .5f;

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
            controller.SetMoveSpeed(aimMoveSpeedFraction);
            controller.SetSprintSpeed(aimMoveSpeedFraction);

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
