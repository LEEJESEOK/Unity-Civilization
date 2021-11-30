using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class PositionAndRotationModifier : CharacterDetector
    {
        public enum CallbackType
        {
            Enter,
            Exit
        }

        [Header("Callbacks")]
        public CallbackType callbackType = CallbackType.Enter;

        [Header("Position")]

        public bool teleport = false;

        [Condition("teleport", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public Transform teleportTarget = null;

        [Header("Rotation")]

        public bool rotate = false;

        [Condition("rotate", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.Hidden)]
        public RotationMode rotationMode = RotationMode.ModifyUp;

        [Condition("rotationMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)RotationMode.ModifyUp)]
        [Tooltip("The target Transform.up vector to use.")]
        public Transform referenceTransform = null;

        [Condition(
            new string[] { "rotationMode", "rotate" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.ConditionType.IsTrue },
            new float[] { (int)RotationMode.AlignWithObject, 0f },
            ConditionAttribute.VisibilityType.Hidden)]
        [Tooltip("The target transform to use as the reference.")]
        public Transform verticalAlignmentReference = null;




        public enum RotationMode
        {
            /// <summary>
            /// 
            /// </summary>
            ModifyUp,
            /// <summary>
            /// 
            /// </summary>
            AlignWithObject
        }

        void Teleport(CharacterActor characterActor)
        {
            if (!teleport)
                return;

            if (teleportTarget == null)
                return;

            Vector3 targetPosition = teleportTarget.position;

            // If the character is 2D, don't change the position z component (Transform).
            if (characterActor.Is2D)
                targetPosition.z = characterActor.transform.position.z;

            characterActor.Teleport(targetPosition);
        }

        void Rotate(CharacterActor characterActor)
        {
            if (!rotate)
                return;

            switch (rotationMode)
            {
                case RotationMode.ModifyUp:

                    break;
                case RotationMode.AlignWithObject:

                    break;
            }

            // Rotate the character
            if (referenceTransform != null)
                characterActor.Up = referenceTransform.up;

            // Just in case the rotation constraint is active ...
            characterActor.upDirectionReference = verticalAlignmentReference;
            characterActor.constraintUpDirection = characterActor.Up;

        }


        protected override void ProcessEnterAction(CharacterActor characterActor)
        {
            if (callbackType != CallbackType.Enter)
                return;

            Teleport(characterActor);
            Rotate(characterActor);
        }

        protected override void ProcessExitAction(CharacterActor characterActor)
        {
            if (callbackType != CallbackType.Exit)
                return;

            Teleport(characterActor);
            Rotate(characterActor);
        }
    }

}
