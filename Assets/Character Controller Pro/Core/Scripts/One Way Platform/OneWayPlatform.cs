using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    [AddComponentMenu("Character Controller Pro/Core/One Way Platform")]
    public class OneWayPlatform : MonoBehaviour
    {
        public LayerMask characterLayerMask = -1;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected Vector3 preSimulationPosition;

        Coroutine postSimulationUpdateCoroutine = null;

        protected Dictionary<Transform, CharacterActor> characters = new Dictionary<Transform, CharacterActor>();

        void Awake()
        {
            colliderComponent = ColliderComponent.CreateInstance(gameObject);
            physicsComponent = PhysicsComponent.CreateInstance(gameObject);
            rigidbodyComponent = RigidbodyComponent.CreateInstance(gameObject);
        }

        void OnEnable()
        {
            if (postSimulationUpdateCoroutine == null)
                postSimulationUpdateCoroutine = StartCoroutine(PostSimulationUpdate());
        }

        void OnDisable()
        {
            if (postSimulationUpdateCoroutine != null)
            {
                StopCoroutine(PostSimulationUpdate());
                postSimulationUpdateCoroutine = null;
            }
        }

        ColliderComponent colliderComponent;
        PhysicsComponent physicsComponent;
        RigidbodyComponent rigidbodyComponent;

        protected int CastPlatformBody(Vector3 castDisplacement)
        {
            float backstepDistance = 0.1f;

            Vector3 castDirection = castDisplacement.normalized;
            castDisplacement += castDirection * backstepDistance;
            Vector3 origin = preSimulationPosition + colliderComponent.Offset - castDirection * backstepDistance;

            HitInfoFilter filter = new HitInfoFilter(characterLayerMask, false, true);
            int hits = physicsComponent.BoxCast(
                origin,
                colliderComponent.BoundsSize,
                castDisplacement,
                rigidbodyComponent.Rotation,
                in filter
            );

            return hits;

        }

        protected bool ValidateOWPCollision(CharacterActor characterActor, Vector3 contactPoint)
        {
            return characterActor.CheckOneWayPlatformCollision(contactPoint, characterActor.Position);
        }

        void FixedUpdate()
        {
            preSimulationPosition = rigidbodyComponent.Position;
        }

        IEnumerator PostSimulationUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;

                Vector3 castDisplacement = rigidbodyComponent.Position - preSimulationPosition;
                int hits = CastPlatformBody(castDisplacement);

                for (int i = 0; i < hits; i++)
                {
                    HitInfo hitInfo = physicsComponent.HitsBuffer[i];

                    if (hitInfo.distance == 0f)
                        continue;

                    CharacterActor characterActor = characters.GetOrRegisterValue<Transform, CharacterActor>(hitInfo.transform);

                    if (characterActor == null)
                        continue;



                    // Ignore the character collider (hitInfo contains the collider information).
                    physicsComponent.IgnoreCollision(in hitInfo, true);

                    if (!characterActor.IsGrounded)
                    {
                        // Check if the collision is valid
                        bool isValidCollision = ValidateOWPCollision(characterActor, hitInfo.point);

                        // If so, then move the character
                        if (isValidCollision)
                        {
                            // How much the actor needs to move            
                            Vector3 actorCastDisplacement = castDisplacement.normalized * (castDisplacement.magnitude - hitInfo.distance);
                            Vector3 destination = characterActor.Position + actorCastDisplacement;

                            // Set the collision filter
                            HitInfoFilter filter = new HitInfoFilter(characterActor.ObstaclesWithoutOWPLayerMask, false, true);

                            characterActor.SweepAndTeleport(destination, in filter);

                            characterActor.ForceGrounded();
                        }


                    }


                }


            }
        }
    }

}
