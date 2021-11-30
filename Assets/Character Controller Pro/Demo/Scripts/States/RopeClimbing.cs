using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class RopeClimbing : CharacterState
    {
        [Header("Movement")]

        [SerializeField]
        protected float climbSpeed = 3f;

        [SerializeField]
        protected float angularSpeed = 120f;

        [SerializeField]
        protected float jumpVelocity = 10f;

        [SerializeField]
        protected float verticalAcceleration = 10f;

        [SerializeField]
        protected float angularAcceleration = 10f;

        [Header("Offset")]

        [SerializeField]
        protected float forwardOffset = -0.25f;

        [Header("Animation")]

        [SerializeField]
        protected string verticalVelocityParameter = "VerticalVelocity";

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        protected Rope currentRope = null;

        protected Dictionary<Transform, Rope> ropes = new Dictionary<Transform, Rope>();

        protected Vector3 verticalVelocity = default(Vector3);
        protected Vector3 angularVelocity = default(Vector3);


        Vector3 ReferencePosition
        {
            get
            {
                return CharacterActor.Top;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Rope[] ropesArray = FindObjectsOfType<Rope>();

            for (int i = 0; i < ropesArray.Length; i++)
                ropes.Add(ropesArray[i].transform, ropesArray[i]);

        }



        public override void CheckExitTransition()
        {
            if (!currentRope.IsInRange(ReferencePosition) || CharacterActions.jump.Started)
                CharacterStateController.EnqueueTransition<NormalMovement>();


        }

        public override bool CheckEnterTransition(CharacterState fromState)
        {

            for (int i = 0; i < CharacterActor.Triggers.Count; i++)
            {
                Trigger trigger = CharacterActor.Triggers[i];

                if (!trigger.firstContact)
                    continue;

                Rope rope = ropes.GetOrRegisterValue<Transform, Rope>(trigger.transform);

                if (rope != null)
                {
                    if (!rope.IsInRange(ReferencePosition))
                        return false;


                    currentRope = rope;

                    return true;
                }

            }

            return false;
        }

        public override void EnterBehaviour(float dt, CharacterState fromState)
        {

            CharacterActor.IsKinematic = false;
            CharacterActor.alwaysNotGrounded = true;
            CharacterActor.UseRootMotion = false;

            CharacterActor.Velocity = Vector3.zero;


            Vector3 closestVectorToRope = ClosestVectorToRope;

            CharacterActor.Forward = closestVectorToRope;

            CharacterActor.Position = CharacterActor.Position + closestVectorToRope + CharacterActor.Forward * forwardOffset;


        }

        Vector3 ClosestVectorToRope
        {
            get
            {
                Vector3 characterToTop = currentRope.TopPosition - CharacterActor.Position;
                return Vector3.ProjectOnPlane(characterToTop, currentRope.BottomToTop);
            }
        }


        public override void ExitBehaviour(float dt, CharacterState toState)
        {

            CharacterActor.alwaysNotGrounded = false;
            currentRope = null;

            if (CharacterActions.jump.Started)
            {
                if (CharacterActions.movement.Detected)
                    CharacterActor.Velocity = CharacterStateController.InputMovementReference * jumpVelocity;
                else
                    CharacterActor.Velocity = CharacterStateController.MovementReferenceForward * jumpVelocity;

                CharacterActor.Forward = Vector3.Normalize(CharacterActor.Velocity);
            }
            else
            {
                CharacterActor.Velocity = Vector3.zero;
            }
        }




        public override void UpdateBehaviour(float dt)
        {
            Vector3 characterPosition = CharacterActor.Position;

            float targetAngularSpeed = CharacterActions.movement.value.x * angularSpeed;


            characterPosition = CustomUtilities.RotatePointAround(
                characterPosition,
                characterPosition + ClosestVectorToRope,
                targetAngularSpeed * dt,
                Vector3.Normalize(currentRope.BottomToTop)
            );

            Vector3 targetAngularVelocity = (characterPosition - CharacterActor.Position) / dt;
            angularVelocity = Vector3.MoveTowards(angularVelocity, targetAngularVelocity, angularAcceleration * dt);


            Vector3 targetVerticalVelocity = CharacterActions.movement.value.y * climbSpeed * CharacterActor.Up;
            verticalVelocity = Vector3.MoveTowards(verticalVelocity, targetVerticalVelocity, verticalAcceleration * dt);

            CharacterActor.Velocity = verticalVelocity + angularVelocity;

        }

        public override void PostUpdateBehaviour(float dt)
        {
            // Always look towards the rope.        
            CharacterActor.Forward = ClosestVectorToRope;

            CharacterStateController.Animator.SetFloat(verticalVelocityParameter, CharacterActor.LocalVelocity.y);
        }




    }


}