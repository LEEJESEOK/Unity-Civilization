using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;


public class ZeroGravity : CharacterState
{
    public float baseSpeed = 10f;
    public float acceleration = 20f;
    public float deceleration = 20f;

    public bool invertRoll = false;
    public bool invertPitch = false;

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
    }


    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        CharacterActor.alwaysNotGrounded = true;
        CharacterActor.UseRootMotion = false;
        CharacterActor.constraintRotation = false;

        targetUp = CharacterActor.Up;
        targetVerticalVelocity = CharacterActor.VerticalVelocity;


    }

    protected Vector3 targetUp = Vector3.up;

    Vector3 targetVerticalVelocity;


    public override void UpdateBehaviour(float dt)
    {
        Vector3 targetVelocity = CharacterStateController.InputMovementReference * baseSpeed;

        CharacterActor.Velocity = Vector3.MoveTowards(CharacterActor.Velocity, targetVelocity, (CharacterActions.movement.Detected ? acceleration : deceleration) * dt);

        // Pitch
        targetUp = Quaternion.AngleAxis(-(invertPitch ? 1f : -1f) * CharacterActions.pitch.value * 180f * dt, CharacterStateController.ExternalReference.right) * targetUp;

        // Roll
        targetUp = Quaternion.AngleAxis((invertRoll ? 1f : -1f) * CharacterActions.roll.value * 180f * dt, CharacterActor.Forward) * targetUp;


        CharacterActor.Up = Vector3.Lerp(CharacterActor.Up, targetUp, 5f * dt);

        CharacterActor.Forward = Vector3.Lerp(CharacterActor.Forward, Vector3.ProjectOnPlane(CharacterStateController.ExternalReference.forward, CharacterActor.Up), 5f * dt);

        if (CharacterActions.jump.value)
        {
            targetVerticalVelocity = CharacterActor.Up * baseSpeed;
            CharacterActor.VerticalVelocity = Vector3.MoveTowards(CharacterActor.VerticalVelocity, targetVerticalVelocity, acceleration * dt);
        }
        else if (CharacterActions.crouch.value)
        {
            targetVerticalVelocity = -CharacterActor.Up * baseSpeed;
            CharacterActor.VerticalVelocity = Vector3.MoveTowards(CharacterActor.VerticalVelocity, targetVerticalVelocity, acceleration * dt);
        }


    }


}
