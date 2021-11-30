using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This struct contains all the character info related to collision, that is, collision flags and external components. All the internal fields are updated frame by frame, and can 
    /// can be accessed by using public properties from the CharacterActor component.
    /// </summary>
    public struct CharacterCollisionInfo
    {
        // Ground
        public Vector3 groundContactPoint;
        public Vector3 groundContactNormal;
        public Vector3 groundStableNormal;
        public float groundSlopeAngle;

        // Head
        public bool headCollision;
        public Contact headContact;
        public float headAngle;

        // Wall
        public bool wallCollision;
        public Contact wallContact;
        public float wallAngle;

        // Edge
        public bool isOnEdge;
        public float edgeAngle;

        // GameObject
        public GameObject groundObject;
        public int groundLayer;

        public Collider groundCollider3D;
        public Collider2D groundCollider2D;
        public Rigidbody groundRigidbody3D;
        public Rigidbody2D groundRigidbody2D;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Resets all the fields to default.
        /// </summary>
        public void Reset()
        {
            ResetGroundInfo();
            ResetWallInfo();
            ResetHeadInfo();
        }



        /// <summary>
        /// Resets the wall contact related info.
        /// </summary>
        public void ResetWallInfo()
        {
            wallCollision = false;
            wallContact = new Contact();
            wallAngle = 0f;
        }

        public void SetWallInfo(in Contact contact, CharacterActor characterActor)
        {
            wallCollision = true;
            wallAngle = Vector3.Angle(characterActor.Up, contact.normal);
            wallContact = contact;
        }

        /// <summary>
        /// Resets the head contact related info.
        /// </summary>
        public void ResetHeadInfo()
        {
            headCollision = false;
            headContact = new Contact();
            headAngle = 0f;
        }

        public void SetHeadInfo(in Contact contact, CharacterActor characterActor)
        {
            headCollision = true;
            headAngle = Vector3.Angle(characterActor.Up, headContact.normal);
            headContact = contact;
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetGroundInfo(in CollisionInfo collisionInfo, CharacterActor characterActor, bool setGroundContact = false)
        {
            if (collisionInfo.hitInfo.hit)
            {
                isOnEdge = collisionInfo.isAnEdge;
                edgeAngle = collisionInfo.edgeAngle;
                groundContactNormal = collisionInfo.contactSlopeAngle < 90f ? collisionInfo.hitInfo.normal : characterActor.Up;
                groundContactPoint = collisionInfo.hitInfo.point;
                groundStableNormal = characterActor.GetGroundSlopeNormal(collisionInfo);
                groundSlopeAngle = Vector3.Angle(characterActor.Up, groundStableNormal);

                groundObject = collisionInfo.hitInfo.transform.gameObject;
                groundLayer = groundObject.layer;
                groundCollider2D = collisionInfo.hitInfo.collider2D;
                groundCollider3D = collisionInfo.hitInfo.collider3D;
                groundRigidbody2D = collisionInfo.hitInfo.rigidbody2D;
                groundRigidbody3D = collisionInfo.hitInfo.rigidbody3D;

                if (setGroundContact)
                    SetGroundContact(collisionInfo, characterActor);
            }
            else
            {
                ResetGroundInfo();
            }
        }

        void SetGroundContact(in CollisionInfo collisionInfo, CharacterActor characterActor)
        {
            if (!collisionInfo.hitInfo.hit)
                return;

            Vector3 point = collisionInfo.hitInfo.point;
            Vector3 normal = collisionInfo.hitInfo.normal;

            Vector3 pointVelocity = Vector3.zero;
            if (characterActor.Is2D)
            {
                if (collisionInfo.hitInfo.rigidbody2D != null)
                    pointVelocity = collisionInfo.hitInfo.rigidbody2D.GetPointVelocity(point);
            }
            else
            {
                if (collisionInfo.hitInfo.rigidbody3D != null)
                    pointVelocity = collisionInfo.hitInfo.rigidbody3D.GetPointVelocity(point);
            }

            Vector3 relativeVelocity = characterActor.Velocity - pointVelocity;

            Contact groundContact = new Contact(point, normal, pointVelocity, relativeVelocity);

            characterActor.PhysicsComponent.Contacts.Add(groundContact);
        }

        public void SetGroundInfo(in Contact contact, CharacterActor characterActor)
        {
            ResetGroundInfo();

            float contactSlopeAngle = Vector3.Angle(characterActor.Up, contact.normal);
            groundContactNormal = contactSlopeAngle < 90f ? contact.normal : characterActor.Up;
            groundContactPoint = contact.point;

            groundStableNormal = contact.normal;

            groundSlopeAngle = Vector3.Angle(characterActor.Up, groundStableNormal);
            groundObject = contact.gameObject;

            if (groundObject != null)
            {
                groundLayer = groundObject.layer;
                groundCollider2D = contact.collider2D;
                groundCollider3D = contact.collider3D;

                if (contact.collider2D != null)
                    groundRigidbody2D = contact.collider2D.attachedRigidbody;

                if (contact.collider3D != null)
                    groundRigidbody3D = contact.collider3D.attachedRigidbody;

            }



        }

        /// <summary>
        /// Resets the ground contact related info.
        /// </summary>
        public void ResetGroundInfo()
        {
            groundContactPoint = Vector3.zero;
            groundContactNormal = Vector3.up;
            groundStableNormal = Vector3.up;

            groundSlopeAngle = 0f;

            isOnEdge = false;
            edgeAngle = 0f;

            groundObject = null;
            groundLayer = 0;
            groundCollider3D = null;
            groundCollider2D = null;
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    }



}
