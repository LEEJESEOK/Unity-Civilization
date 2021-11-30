// #define EDGE_DETECTOR_SPHERE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;


namespace Lightbug.CharacterControllerPro.Core
{

    public struct RayArrayInfo
    {
        public float averageDistance;
        public Vector3 averageNormal;
    }

    // IMPORTANT: This class needs to be serializable in order to be compatible with assembly reloads.
    [System.Serializable]
    public class CharacterCollisions
    {

        CharacterActor characterActor = null;
        PhysicsComponent physicsComponent = null;

        CollisionInfo collisionInfo = new CollisionInfo();

        public void Initialize(CharacterActor characterActor, PhysicsComponent physicsComponent)
        {
            this.characterActor = characterActor;
            this.physicsComponent = physicsComponent;
        }

        // Important: PhysX (3D Physics) does not use the "contact offset" for depenetration purposes (like 2D does). Instead, it defines an internal restDistance which is then 
        // used to eliminate penetration between two bodies. Contact generation is still defined by the contact offset.
        // On the other hand, Box2D uses the "contact offset" for both contact generation and de-penetration.
        // This means that 3D collision will be handled using the "skin width" concept (collision shape smaller than the collider). For 2D both collision shape and collider are the same.

        public float ContactOffset => characterActor.Is2D ? Physics2D.defaultContactOffset : CharacterConstants.SkinWidth;
        public float CollisionRadius => characterActor.Is2D ? characterActor.BodySize.x / 2f : characterActor.BodySize.x / 2f - ContactOffset;
        float BackstepDistance => 2f * ContactOffset;

        /// <summary>
        /// Checks vertically for the ground using a SphereCast.
        /// </summary>
        public CollisionInfo CheckForGround(Vector3 position, float stepOffset, float stepDownDistance, in HitInfoFilter hitInfoFilter)
        {

            float preDistance = stepOffset + BackstepDistance;
            Vector3 displacement = CustomUtilities.Multiply(-characterActor.Up, Mathf.Max(CharacterConstants.GroundCheckDistance, stepDownDistance));

            Vector3 castDisplacement = characterActor.Is2D ?
                displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance + ContactOffset) :
                displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance);

            Vector3 origin = characterActor.GetBottomCenter(position, preDistance);

            physicsComponent.SphereCast(
                out HitInfo hitInfo,
                origin,
                CollisionRadius,
                castDisplacement,
                in hitInfoFilter
            );

            UpdateCollisionInfo(collisionInfo, position, in hitInfo, displacement, castDisplacement, preDistance, false, true, in hitInfoFilter);


            return collisionInfo;
        }



        /// <summary>
        /// Checks vertically for the ground using a Raycast.
        /// </summary>
        public CollisionInfo CheckForGroundRay(Vector3 position, in HitInfoFilter hitInfoFilter)
        {

            float preDistance = characterActor.BodySize.x / 2f;

            // GroundCheckDistance is not high enough (usually), especially if the character is on top of a slope.
            Vector3 displacement = CustomUtilities.Multiply(-characterActor.Up, Mathf.Max(CharacterConstants.GroundCheckDistance, characterActor.stepDownDistance));

            Vector3 castDisplacement = displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance);

            Vector3 origin = characterActor.GetBottomCenter(position);

            HitInfo hitInfo;

#if EDGE_DETECTOR_SPHERE
		physicsComponent.SphereCast(
			out hitInfo ,
			origin ,
			0.01f ,
			castDisplacement ,
			in hitInfoFilter
		);		
#else
            if (characterActor.Is2D)
            {
                physicsComponent.Raycast(
                    out hitInfo,
                    origin,
                    castDisplacement,
                    in hitInfoFilter
                );
            }
            else
            {
                physicsComponent.SimpleRaycast(
                    out hitInfo,
                    origin,
                    castDisplacement,
                    in hitInfoFilter
                );
            }


#endif

            UpdateCollisionInfo(collisionInfo, position, in hitInfo, displacement, castDisplacement, preDistance, false, false, in hitInfoFilter);

            return collisionInfo;


        }


        public CollisionInfo CastBody(Vector3 position, Vector3 displacement, float bottomOffset, in HitInfoFilter hitInfoFilter)
        {

            float preDistance = BackstepDistance;

            Vector3 bottom = characterActor.GetBottomCenter(position, bottomOffset);
            Vector3 top = characterActor.GetTopCenter(position);

            bottom -= CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance);
            top -= CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance);


            Vector3 castDisplacement = characterActor.Is2D ?
                displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance + ContactOffset) :
                displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance);

            int hits = 0;

            hits = physicsComponent.CapsuleCast(
                out HitInfo hitInfo,
                bottom,
                top,
                CollisionRadius,
                castDisplacement,
                in hitInfoFilter
            );

            UpdateCollisionInfo(collisionInfo, position, in hitInfo, displacement, castDisplacement, preDistance, false, false, in hitInfoFilter);


            return collisionInfo;
        }


        /// <summary>
        /// Checks if the character is currently overlapping with any obstacle from a given layermask.
        /// </summary>
        public bool CheckOverlapWithLayerMask(Vector3 position, float bottomOffset, in HitInfoFilter hitInfoFilter)
        {

            Vector3 bottom = characterActor.GetBottomCenter(position, bottomOffset);
            Vector3 top = characterActor.GetTopCenter(position);
            float radius = characterActor.BodySize.x / 2f;

            bool overlap = physicsComponent.OverlapCapsule(
                bottom,
                top,
                radius,
                in hitInfoFilter
            );

            return overlap;
        }

        /// <summary>
        /// Checks if the character size fits at a specific location.
        /// </summary>
        public bool CheckBodySize(Vector3 size, Vector3 position, in HitInfoFilter hitInfoFilter)
        {

            Vector3 bottom = characterActor.GetBottomCenter(position, size);
            Vector3 top = characterActor.GetTopCenter(position, size);
            float radius = size.x / 2f;

            // GetBottomCenterToTopCenter ) ---> Up

            Vector3 castDisplacement = characterActor.GetBottomCenterToTopCenter(size);

            HitInfo hitInfo;
            physicsComponent.SphereCast(
                out hitInfo,
                bottom,
                radius,
                castDisplacement,
                in hitInfoFilter
            );


            bool overlap = hitInfo.hit;

            return !overlap;
        }

        /// <summary>
        /// Checks if the character size fits in place.
        /// </summary>
        public bool CheckBodySize(Vector3 size, in HitInfoFilter hitInfoFilter)
        {
            return CheckBodySize(size, characterActor.Position, in hitInfoFilter);
        }


        public void UpdateCollisionInfo(
            CollisionInfo collisionInfo, Vector3 position, in HitInfo hitInfo, Vector3 displacement, Vector3 castDisplacement,
            float preDistance, bool useContactNormal, bool calculateEdge = true, in HitInfoFilter hitInfoFilter = new HitInfoFilter()
        )
        {

            if (hitInfo.hit)
            {
                Vector3 characterUp = characterActor.Up;
                Vector3 castDirection = Vector3.Normalize(castDisplacement);

                displacement = CustomUtilities.Multiply(castDirection, hitInfo.distance - preDistance - ContactOffset);

                if (calculateEdge)
                {
                    Vector3 edgeCenterReference = characterActor.GetBottomCenter(position + displacement, 0f);
                    UpdateEdgeInfo(in edgeCenterReference, in hitInfo.point, in hitInfoFilter, out HitInfo upperHitInfo, out HitInfo lowerHitInfo);

                    collisionInfo.SetData(in hitInfo, characterUp, displacement, in upperHitInfo, in lowerHitInfo);
                }
                else
                {
                    collisionInfo.SetData(in hitInfo, characterUp, displacement);
                }
            }
            else
            {
                collisionInfo.Reset();
            }

        }

        void UpdateEdgeInfo(in Vector3 edgeCenterReference, in Vector3 contactPoint, in HitInfoFilter hitInfoFilter, out HitInfo upperHitInfo, out HitInfo lowerHitInfo)
        {
            Vector3 castDirection = (contactPoint - edgeCenterReference);
            castDirection.Normalize();

            Vector3 castDisplacement = CustomUtilities.Multiply(castDirection, CharacterConstants.EdgeRaysCastDistance);

            Vector3 upperHitPosition = edgeCenterReference + CustomUtilities.Multiply(characterActor.Up, CharacterConstants.EdgeRaysSeparation);
            Vector3 lowerHitPosition = edgeCenterReference - CustomUtilities.Multiply(characterActor.Up, CharacterConstants.EdgeRaysSeparation);

#if EDGE_DETECTOR_SPHERE
		physicsComponent.SphereCast(
			out upperHitInfo,
			upperHitPosition ,
			0.5f * CharacterConstants.EdgeRaysSeparation ,
			castDisplacement ,
			in hitInfoFilter
		);
#else
            if (characterActor.Is2D)
            {
                physicsComponent.Raycast(
                    out upperHitInfo,
                    upperHitPosition,
                    castDisplacement,
                    in hitInfoFilter
                );
            }
            else
            {
                physicsComponent.SimpleRaycast(
                    out upperHitInfo,
                    upperHitPosition,
                    castDisplacement,
                    in hitInfoFilter
                );
            }

#endif


#if EDGE_DETECTOR_SPHERE
		physicsComponent.SphereCast(
			out lowerHitInfo,
			lowerHitPosition ,
			0.5f * CharacterConstants.EdgeRaysSeparation ,
			castDisplacement ,
			in hitInfoFilter
		);		
#else
            if (characterActor.Is2D)
            {
                physicsComponent.Raycast(
                    out lowerHitInfo,
                    lowerHitPosition,
                    castDisplacement,
                    in hitInfoFilter
                );
            }
            else
            {
                physicsComponent.SimpleRaycast(
                    out lowerHitInfo,
                    lowerHitPosition,
                    castDisplacement,
                    in hitInfoFilter
                );
            }
#endif



        }
    }

}