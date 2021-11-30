using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{
    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Wall Slide")]
    public class WallSlide : CharacterState
    {

        [Header("Filter")]

        [SerializeField]
        protected bool filterByTag = true;

        [Condition("filterByTag", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        protected string wallTag = "WallSlide";


        [Header("Slide")]

        [SerializeField]
        protected float slideAcceleration = 10f;

        [Range(0f, 1f)]
        [SerializeField]
        protected float initialIntertia = 0.4f;

        [Header("Grab")]

        public bool enableGrab = true;

        public bool enableClimb = true;

        [Condition("enableClimb", ConditionAttribute.ConditionType.IsTrue)]
        public float wallClimbHorizontalSpeed = 1f;

        [Condition("enableClimb", ConditionAttribute.ConditionType.IsTrue)]
        public float wallClimbVerticalSpeed = 3f;

        [Condition("enableClimb", ConditionAttribute.ConditionType.IsTrue)]
        public float wallClimbAcceleration = 10f;



        [Header("Size")]

        [SerializeField]
        protected bool modifySize = true;

        [Condition("modifySize", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        protected float height = 1.5f;

        [Header("Jump")]

        [SerializeField]
        protected float jumpNormalVelocity = 5f;

        [SerializeField]
        protected float jumpVerticalVelocity = 10f;

        [Header("Animation")]

        [SerializeField]
        protected string horizontalVelocityParameter = "HorizontalVelocity";

        [SerializeField]
        protected string verticalVelocityParameter = "VerticalVelocity";

        [SerializeField]
        protected string grabParameter = "Grab";

        [SerializeField]
        protected string movementDetectedParameter = "MovementDetected";



        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected bool wallJump = false;
        protected Vector2 initialSize = Vector2.zero;

        public override void CheckExitTransition()
        {
            if (CharacterActions.crouch.value || CharacterActor.IsGrounded || !CharacterActor.WallCollision || !CheckCenterRay())
            {
                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
            else if (CharacterActions.jump.Started)
            {
                wallJump = true;

                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
            else
            {
                CharacterStateController.EnqueueTransition<LedgeHanging>();
            }
        }


        public override bool CheckEnterTransition(CharacterState fromState)
        {
            if (CharacterActor.IsAscending)
                return false;

            if (!CharacterActor.WallCollision)
                return false;

            if (filterByTag)
                if (!CharacterActor.WallContact.gameObject.CompareTag(wallTag))
                    return false;

            if (!CheckCenterRay())
                return false;


            return true;
        }

        protected virtual bool CheckCenterRay()
        {
            HitInfoFilter filter = new HitInfoFilter(
                CharacterActor.PhysicsComponent.CollisionLayerMask,
                true,
                true
            );

            CharacterActor.PhysicsComponent.Raycast(
                out HitInfo centerRayHitInfo,
                CharacterActor.Center,
                -CharacterActor.WallContact.normal * 1.2f * CharacterActor.BodySize.x,
                in filter
            );

            return centerRayHitInfo.hit && centerRayHitInfo.transform.gameObject == CharacterActor.WallContact.gameObject;
        }



        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            CharacterActor.UseRootMotion = false;

            CharacterActor.Velocity *= initialIntertia;
            CharacterActor.Forward = -CharacterActor.WallContact.normal;



            if (modifySize)
            {
                initialSize = CharacterActor.BodySize;
                CharacterActor.SetBodySize(new Vector2(initialSize.x, height));
            }
        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            if (wallJump)
            {
                wallJump = false;

                // Apply the wall jump velocity.
                CharacterActor.Velocity = jumpVerticalVelocity * CharacterActor.Up + jumpNormalVelocity * CharacterActor.WallContact.normal;

                // Do a 180 degrees turn.
                CharacterActor.SetYaw(180f);
            }

            if (modifySize)
            {
                CharacterActor.SetBodySize(initialSize);
            }


        }

        protected bool IsGrabbing => CharacterActions.run.value && enableGrab;



        public override void UpdateBehaviour(float dt)
        {
            if (IsGrabbing)
            {
                Vector3 rightDirection = Vector3.ProjectOnPlane(CharacterStateController.MovementReferenceRight, CharacterActor.WallContact.normal);
                rightDirection.Normalize();

                Vector3 upDirection = CharacterActor.Up;

                Vector3 targetVelocity = enableClimb ? CharacterActions.movement.value.x * rightDirection * wallClimbHorizontalSpeed +
                CharacterActions.movement.value.y * upDirection * wallClimbVerticalSpeed : Vector3.zero;

                CharacterActor.Velocity = Vector3.MoveTowards(CharacterActor.Velocity, targetVelocity, wallClimbAcceleration * dt);

            }
            else
            {
                CharacterActor.VerticalVelocity += -CharacterActor.Up * slideAcceleration * dt;
            }
        }

        public override void PostUpdateBehaviour(float dt)
        {
            CharacterStateController.Animator.SetFloat(horizontalVelocityParameter, CharacterActor.LocalVelocity.x);
            CharacterStateController.Animator.SetFloat(verticalVelocityParameter, CharacterActor.LocalVelocity.y);
            CharacterStateController.Animator.SetBool(grabParameter, IsGrabbing);
            CharacterStateController.Animator.SetBool(movementDetectedParameter, CharacterActions.movement.Detected);

        }

        public override void UpdateIK(int layerIndex)
        {
            if (IsGrabbing && CharacterActions.movement.Detected)
            {
                CharacterStateController.Animator.SetLookAtWeight(Mathf.Clamp01(CharacterActor.Velocity.magnitude), 0f, 0.2f);
                CharacterStateController.Animator.SetLookAtPosition(CharacterActor.Position + CharacterActor.Velocity);
            }
            else
            {
                CharacterStateController.Animator.SetLookAtWeight(0f);
            }

        }







    }

}
