using Lightbug.CharacterControllerPro.Core;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class JumpPad : CharacterDetector
    {
        public float jumpPadVelocity = 10f;

        protected override void ProcessEnterAction(CharacterActor characterActor)
        {
            if (characterActor.GroundObject != gameObject)
                return;

            characterActor.ForceNotGrounded();
            characterActor.Velocity += transform.up * jumpPadVelocity;
        }

        protected override void ProcessStayAction(CharacterActor characterActor)
        {
            ProcessEnterAction(characterActor);
        }

    }

}
