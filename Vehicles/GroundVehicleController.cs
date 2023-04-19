using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundVehicleController : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    [SerializeField] private WheelCollider _frontWheelLeft, _frontWheelRight;
    [SerializeField] private WheelCollider _rearWheelLeft, _rearWheelRight;
    [SerializeField] private Transform t_frontWheelLeft, t_frontWheelRight;
    [SerializeField] private Transform t_rearWheelLeft, t_rearWheelRight;
    [SerializeField] private float _maxSteerAngle = 30f;
    [SerializeField] private float _motorForce = 50f;

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPositions();
    }

    public void GetInput() 
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    public void Steer() 
    {
        m_steeringAngle = _maxSteerAngle * m_horizontalInput;
        _frontWheelLeft.steerAngle = m_steeringAngle;
        _frontWheelRight.steerAngle = m_steeringAngle;
    }

    public void Accelerate() 
    {
        _frontWheelLeft.motorTorque = m_verticalInput * _motorForce;
        _frontWheelRight.motorTorque = m_verticalInput * _motorForce;
        _rearWheelLeft.motorTorque = m_verticalInput * _motorForce;
        _rearWheelRight.motorTorque = m_verticalInput * _motorForce;
    }

    private void UpdateWheelPositions() 
    {
        UpdateWheelPose(_frontWheelLeft, t_frontWheelLeft);
        UpdateWheelPose(_frontWheelRight, t_frontWheelRight);
        UpdateWheelPose(_rearWheelLeft, t_rearWheelLeft);
        UpdateWheelPose(_rearWheelRight, t_rearWheelRight);
    }

    private void UpdateWheelPose(WheelCollider wheel, Transform transform) 
    {
        Vector3 position = transform.position;
        Quaternion quaternion = transform.rotation;

        wheel.GetWorldPose(out position, out quaternion);

        transform.position = position;
        transform.rotation = quaternion;
    }

    private void Break() 
    {
        if (Input.GetKey(KeyCode.Space)) 
        {

        }
    }


}
