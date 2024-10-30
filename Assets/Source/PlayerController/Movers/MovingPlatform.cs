﻿
using KinematicCharacterController;

using UnityEngine;


public class MovingPlatform : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;

    public Vector3 TranslationAxis = Vector3.right;
    public float TranslationPeriod = 10;
    public float TranslationSpeed = 1;
    public Vector3 RotationAxis = Vector3.up;
    public float RotSpeed = 10;
    public Vector3 OscillationAxis = Vector3.zero;
    public float OscillationPeriod = 10;
    public float OscillationSpeed = 10;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalPosition = _originalPosition + (TranslationAxis.normalized * Mathf.Sin(Time.time * TranslationSpeed) * TranslationPeriod);

        goalRotation = _originalRotation;
    }

    private void Start()
    {
        _originalPosition = Mover.Rigidbody.position;
        _originalRotation = Mover.Rigidbody.rotation;

        Mover.MoverController = this;
    }
}

