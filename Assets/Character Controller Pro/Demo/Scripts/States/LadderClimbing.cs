using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Ladder Climbing")]
    public class LadderClimbing : CharacterState
    {



        [Header("Offset")]

        [SerializeField]
        protected bool useIKOffsetValues = false;

        [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        protected Vector3 rightFootOffset = Vector3.zero;

        [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        protected Vector3 leftFootOffset = Vector3.zero;

        [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        protected Vector3 rightHandOffset = Vector3.zero;

        [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        protected Vector3 leftHandOffset = Vector3.zero;

        [Header("Activation")]

        [SerializeField]
        protected bool useInteractAction = true;

        [SerializeField]
        protected bool filterByAngle = true;

        [SerializeField]
        protected float maxAngle = 70f;

        [Header("Animation")]

        [SerializeField]
        protected string bottomDownParameter = "BottomDown";

        [SerializeField]
        protected string bottomUpParameter = "BottomUp";

        [SerializeField]
        protected string topDownParameter = "TopDown";

        [SerializeField]
        protected string topUpParameter = "TopUp";

        [SerializeField]
        protected string upParameter = "Up";

        [SerializeField]
        protected string downParameter = "Down";

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected Dictionary<Transform, Ladder> ladders = new Dictionary<Transform, Ladder>();

        public enum LadderClimbState
        {
            Entering,
            AnimationBased,
        }

        protected LadderClimbState state;
        protected Ladder currentLadder = null;
        protected Vector3 targetPosition = Vector3.zero;
        protected int currentClimbingAnimation = 0;
        protected bool forceExit = false;




        public Dictionary<Transform, Ladder> Ladders
        {
            get
            {
                return ladders;
            }
        }

        public override void CheckExitTransition()
        {
            if (forceExit)
                CharacterStateController.EnqueueTransition<NormalMovement>();
        }

        public override bool CheckEnterTransition(CharacterState fromState)
        {

            if (!CharacterActor.IsGrounded)
                return false;

            if (useInteractAction && !CharacterActions.interact.Started)
                return false;

            for (int i = 0; i < CharacterActor.Triggers.Count; i++)
            {
                Trigger trigger = CharacterActor.Triggers[i];

                Ladder ladder = ladders.GetOrRegisterValue<Transform, Ladder>(trigger.transform);

                if (ladder != null)
                {
                    if (!useInteractAction && CharacterActor.WasGrounded && !trigger.firstContact)
                    {
                        return false;
                    }

                    currentLadder = ladder;

                    // Check if the character is closer to the top of the ladder.
                    float distanceToTop = Vector3.Distance(CharacterActor.Position, currentLadder.TopReference.position);
                    float distanceToBottom = Vector3.Distance(CharacterActor.Position, currentLadder.BottomReference.position);

                    isBottom = distanceToBottom < distanceToTop;

                    if (filterByAngle)
                    {
                        Vector3 ladderToCharacter = CharacterActor.Position - currentLadder.transform.position;
                        ladderToCharacter = Vector3.ProjectOnPlane(ladderToCharacter, currentLadder.transform.up);

                        float angle = Vector3.Angle(currentLadder.FacingDirectionVector, ladderToCharacter);

                        if (isBottom)
                        {
                            if (angle >= maxAngle)
                                return true;
                            else
                                continue;
                        }
                        else
                        {
                            if (angle <= maxAngle)
                                return true;
                            else
                                continue;
                        }


                    }
                    else
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        protected bool isBottom = false;


        public override void EnterBehaviour(float dt, CharacterState fromState)
        {

            // Making the character kinematic here is going to trigger a new Exit/Enter call in the next physics step.
            // The PhysicsComponent handles this change (OnBodyChange event).
            CharacterActor.IsKinematic = true;

            CharacterActor.alwaysNotGrounded = true;
            CharacterActor.Velocity = Vector3.zero;

            currentClimbingAnimation = isBottom ? 0 : currentLadder.ClimbingAnimations;

            targetPosition = isBottom ? currentLadder.BottomReference.position : currentLadder.TopReference.position;

            CharacterActor.Forward = currentLadder.FacingDirectionVector;

            state = LadderClimbState.Entering;


        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            forceExit = false;
            CharacterActor.IsKinematic = false;
            CharacterActor.alwaysNotGrounded = false;
            currentLadder = null;

            CharacterStateController.ResetIKWeights();

            CharacterActor.Velocity = Vector3.zero;
        }

        protected override void Awake()
        {
            base.Awake();

            Ladder[] laddersArray = FindObjectsOfType<Ladder>();
            for (int i = 0; i < laddersArray.Length; i++)
                ladders.Add(laddersArray[i].transform, laddersArray[i]);
        }

        protected override void Start()
        {
            base.Start();

            if (CharacterStateController.Animator == null)
            {
                Debug.Log("The LadderClimbing state needs the character to have a reference to an Animator component. Destroying this state...");
                Destroy(this);
            }

        }


        public override void UpdateBehaviour(float dt)
        {


            switch (state)
            {
                case LadderClimbState.Entering:

                    CharacterActor.Position = targetPosition;

                    // Root motion
                    CharacterActor.SetUpRootMotion(
                        true,
                        PhysicsActor.RootMotionVelocityType.SetVelocity,
                        false
                    );
                    CharacterStateController.Animator.SetTrigger(isBottom ? bottomUpParameter : topDownParameter);

                    state = LadderClimbState.AnimationBased;

                    break;

                case LadderClimbState.AnimationBased:

                    if (CharacterStateController.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        if (CharacterActions.interact.Started)
                        {
                            if (useInteractAction)
                                forceExit = true;
                        }
                        else
                        {
                            if (CharacterActions.movement.Up)
                            {
                                if (currentClimbingAnimation == currentLadder.ClimbingAnimations)
                                {
                                    CharacterStateController.Animator.SetTrigger(topUpParameter);
                                }
                                else
                                {
                                    CharacterStateController.Animator.SetTrigger(upParameter);
                                    currentClimbingAnimation++;
                                }
                            }
                            else if (CharacterActions.movement.Down)
                            {
                                if (currentClimbingAnimation == 0)
                                {
                                    CharacterStateController.Animator.SetTrigger(bottomDownParameter);
                                }
                                else
                                {
                                    CharacterStateController.Animator.SetTrigger(downParameter);
                                    currentClimbingAnimation--;
                                }
                            }


                        }

                    }
                    else if (CharacterStateController.Animator.GetCurrentAnimatorStateInfo(0).IsName("Entry"))
                    {
                        forceExit = true;
                        CharacterActor.ForceGrounded();
                    }

                    break;

            }
        }


        public override void UpdateIK(int layerIndex)
        {
            if (!useIKOffsetValues)
                return;

            UpdateIKElement(AvatarIKGoal.LeftFoot, leftFootOffset);
            UpdateIKElement(AvatarIKGoal.RightFoot, rightFootOffset);
            UpdateIKElement(AvatarIKGoal.LeftHand, leftHandOffset);
            UpdateIKElement(AvatarIKGoal.RightHand, rightHandOffset);

        }

        void UpdateIKElement(AvatarIKGoal avatarIKGoal, Vector3 offset)
        {
            // Get the original (weight = 0) ik position.
            CharacterStateController.Animator.SetIKPositionWeight(avatarIKGoal, 0f);
            Vector3 originalRightFootPosition = CharacterStateController.Animator.GetIKPosition(avatarIKGoal);

            // Affect the original ik position with the offset.
            CharacterStateController.Animator.SetIKPositionWeight(avatarIKGoal, 1f);
            CharacterStateController.Animator.SetIKPosition(avatarIKGoal, originalRightFootPosition + offset);
        }


    }

}
