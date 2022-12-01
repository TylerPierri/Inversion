using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Data/Player/Movement", order = 1)]
public class playerMovementData : ScriptableObject
{
    public float baseMovementSpeed = 12f;
    public float sprintMovementSpeed = 12f;
    public float accelerationSpeed = 5.0f;

    public float jumpForce = 20f;
    public float jumpTimer = 0.4f;

    public float fallMultiplier = 2.5f;

    public float idleTime = 0.3f;
}
