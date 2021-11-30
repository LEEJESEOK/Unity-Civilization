using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public enum CharacterActorState
    {
        NotGrounded,
        StableGrounded,
        UnstableGrounded
    }


    /// <summary>
    /// This class represents a character actor. It contains all the character information, collision flags, collision events, and so on. It also responsible for the execution order 
    /// of everything related to the character, such as movement, rotation, teleportation, rigidbodies interactions, body size, etc. Since the character can be 2D or 3D, this abstract class must be implemented in the 
    /// two formats, one for 2D and one for 3D.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Actor")]
    [RequireComponent(typeof(CharacterBody))]
    [DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder)]
    public class CharacterActor : PhysicsActor
    {
        [Header("Collision")]

        [Tooltip("One way platforms are objects that can be contacted by the character feet (bottom sphere) while descending.")]
        public LayerMask oneWayPlatformsLayerMask = 0;

        [Tooltip("This value defines (in degrees) the total arc used by the one way platform detection algorithm (using the bottom part of the capsule). " +
        "the angle is measured between the up direction and the segment formed by the contact point and the character bottom center (capsule). " +
        "\nArc = 180 degrees ---> contact point = any point on the bottom sphere." +
        "\nArc = 0 degrees ---> contact point = bottom most point")]
        [Range(0f, 179f)]
        public float oneWayPlatformsValidArc = 175f;

        [Tooltip("This option will enable a trigger (located at the capsule bottom center) that can be used to generate OnTriggerXXX messages. " +
        "Normally the character won't generate these messages (OnCollisionXXX/OnTriggerXXX) since the collider is not making direct contact with the ground")]
        public bool useGroundTrigger = false;

        [Header("Grounding")]

        [Tooltip("Prevents the character from enter grounded state (IsGrounded will be false)")]
        public bool alwaysNotGrounded = false;

        [Condition("alwaysNotGrounded", ConditionAttribute.ConditionType.IsFalse)]
        [Tooltip("If enabled the character will do an initial ground check (at \"Start\"). If the test fails the starting state will be \"Not grounded\".")]
        public bool forceGroundedAtStart = true;

        [Space(10f)]

        [Tooltip("Objects NOT represented by this layer mask will be considered by the character as \"unstable objects\". " +
        "If you don't want to define unstable layers, select \"Everything\" (default value).")]
        public LayerMask stableLayerMask = -1;

        [Tooltip("If the character is stable, the ground slope angle must be less than or equal to this value in order to remain \"stable\". " +
        "The angle is calculated using the \"ground stable normal\".")]
        [Range(1f, 89f)]
        public float slopeLimit = 55f;


        [Tooltip("Situation: The character makes contact with the ground and detect a stable edge." +
        "\n\n True: the character will enter stable state regardless of the collision contact angle.\n" +
        "\n\n False: the character will use the contact angle instead (contact normal) in order to determine stability (<= slopeLimit).")]
        public bool useStableEdgeWhenLanding = true;

        [Tooltip("The offset distance applied to the bottom of the character. A higher offset means more walkable surfaces")]
        [Min(0f)]
        public float stepUpDistance = 0.5f;

        [Tooltip("The distance the character is capable of detecting ground. Use this value to clamp (or not) the character to the ground.")]
        [Min(0f)]
        public float stepDownDistance = 0.5f;

        [Space(10f)]


        [Tooltip("With this enabled the character bottom sphere (capsule) will be simulated as a cylinder. This works only when the character is standing on an edge.")]
        public bool edgeCompensation = false;

        [Header("Unstable ground")]

        [Tooltip("Should the character detect a new (and valid) ground if its vertical velocity is positive?")]
        public bool detectGroundWhileAscending = false;


        [Tooltip("A high planar velocity value (e.g. tight platformer) can be projected onto an unstable surface, causing the character to " +
            "climb over obstacles it is not supposed to. This option will prevent that by removing all the extra planar velocity.")]
        public bool preventUnstableClimbing = true;

        [Tooltip("This will prevent the character from stepping over an unstable surface (a \"bad\" step). This requires a bit more processing, so if your character does not need this level of precision " +
        "you can disable it.")]
        public bool preventBadSteps = true;


        [Space(10f)]


        [Header("Dynamic ground")]

        [Tooltip("Should the character be affected by the movement of the ground?")]
        public bool supportDynamicGround = true;

        public LayerMask dynamicGroundLayerMask = -1;

        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("The forward direction of the character will be affected by the rotation of the ground (only yaw motion allowed).")]
        public bool rotateForwardDirection = true;

        [Space(10f)]

        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("This is the maximum ground velocity delta (from the previous frame to the current one) tolerated by the character." +
        "\n\nIf the ground accelerates too much, then the character will stop moving with it." + "\n\nImportant: This does not apply to one way platforms.")]
        public float maxGroundVelocityChange = 30f;

        [Space(8f)]

        [UnityEngine.Serialization.FormerlySerializedAs("maxForceNotGroundedGroundVelocity")]
        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("When the character becomes \"not grounded\" (after a ForceNotGrounded call) part of the ground velocity can be transferred to its own velocity. " +
        "This value represents the minimum planar velocity required.")]
        public float inheritedGroundPlanarVelocityThreshold = 2f;

        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("When the character becomes \"not grounded\" (after a ForceNotGrounded call) part of the ground velocity can be transferred to its own velocity. " +
        "This value represents how much of the planar component is utilized.")]
        public float inheritedGroundPlanarVelocityMultiplier = 1f;

        [Space(8f)]
        [UnityEngine.Serialization.FormerlySerializedAs("maxForceNotGroundedGroundVelocity")]
        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("When the character becomes \"not grounded\" (after a ForceNotGrounded call) part of the ground velocity can be transferred to its own velocity. " +
        "This value represents the minimum vertical velocity required.")]
        public float inheritedGroundVerticalVelocityThreshold = 2f;

        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("When the character becomes \"not grounded\" (after a ForceNotGrounded call) part of the ground velocity can be transferred to its own velocity. " +
        "This value represents how much of the planar component is utilized.")]
        public float inheritedGroundVerticalVelocityMultiplier = 1f;

        [Header("Velocity")]

        [Tooltip("Whether or not to project the initial velocity (stable) onto walls.")]
        [SerializeField]
        public bool slideOnWalls = true;

        [Tooltip("Should the actor re-assign the rigidbody velocity after the simulation is done?\n\n" +
        "PreSimulationVelocity: the character uses the velocity prior to the simulation (modified by this component).\nPostSimulationVelocity: the character uses the velocity received from the simulation (no re-assignment).\nInputVelocity: the character \"gets back\" its initial velocity (before being modified by this component).")]
        public CharacterVelocityMode stablePostSimulationVelocity = CharacterVelocityMode.UsePostSimulationVelocity;

        [Tooltip("Should the actor re-assign the rigidbody velocity after the simulation is done?\n\n" +
        "PreSimulationVelocity: the character uses the velocity prior to the simulation (modified by this component).\nPostSimulationVelocity: the character uses the velocity received from the simulation (no re-assignment).\nInputVelocity: the character \"gets back\" its initial velocity (before being modified by this component).")]
        public CharacterVelocityMode unstablePostSimulationVelocity = CharacterVelocityMode.UsePostSimulationVelocity;


        [Header("Size")]

        [Tooltip("This field determines a fixed point (top, center or bottom) that will be used as a reference during size changes. " +
        "For instance, by using \"top\" as a reference, the character will shrink/grow my modifying the bottom part of the body (the top part will not move)." +
        "\n\nImportant: For a \"grounded\" character only the \"bottom\" reference is available.")]
        public SizeReferenceType sizeReferenceType = SizeReferenceType.Bottom;

        [Min(0f)]
        public float sizeLerpSpeed = 8f;

        [Header("Rotation")]

        [Tooltip("Should this component define the character \"Up\" direction?")]
        public bool constraintRotation = true;

        [Condition("constraintRotation", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public Transform upDirectionReference = null;

        [Condition(
            new string[] { "constraintRotation", "upDirectionReference" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.ConditionType.IsNotNull },
            new float[] { 0f, 0f },
            ConditionAttribute.VisibilityType.Hidden)]
        public VerticalAlignmentSettings.VerticalReferenceMode upDirectionReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;

        [Condition(
            new string[] { "constraintRotation", "upDirectionReference" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.ConditionType.IsNull },
            new float[] { 0f, 0f },
            ConditionAttribute.VisibilityType.Hidden)]
        [Tooltip("The desired up direction.")]
        public Vector3 constraintUpDirection = Vector3.up;






        [Header("Physics")]

        public bool canPushDynamicRigidbodies = true;

        [Condition("canPushDynamicRigidbodies", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public LayerMask pushableRigidbodyLayerMask = -1;

        public bool applyWeightToGround = true;

        [Condition("applyWeightToGround", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public float weightGravity = CharacterConstants.DefaultGravity;




        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────



        public float StepOffset
        {
            get
            {
                return stepUpDistance - BodySize.x / 2f;
            }
        }


        public void OnValidate()
        {
            if (CharacterBody == null)
                CharacterBody = GetComponent<CharacterBody>();

            stepUpDistance = Mathf.Clamp(
                stepUpDistance,
                CharacterConstants.ColliderMinBottomOffset + CharacterBody.BodySize.x / 2f,
                CharacterBody.BodySize.y - CharacterBody.BodySize.x / 2f
            );

            CustomUtilities.SetPositive(ref maxGroundVelocityChange);
            CustomUtilities.SetPositive(ref inheritedGroundPlanarVelocityThreshold);
            CustomUtilities.SetPositive(ref inheritedGroundPlanarVelocityMultiplier);
            CustomUtilities.SetPositive(ref inheritedGroundVerticalVelocityThreshold);
            CustomUtilities.SetPositive(ref inheritedGroundVerticalVelocityMultiplier);
            // CustomUtilities.SetMax( ref unstableToStableSlopeLimit , slopeLimit );

        }

        /// <summary>
        /// Sets up root motion for this actor.
        /// </summary>
        public void SetUpRootMotion(
            bool updateRootPosition = true,
            bool updateRootRotation = true
        )
        {
            UseRootMotion = true;
            UpdateRootPosition = updateRootPosition;
            UpdateRootRotation = updateRootRotation;
        }

        /// <summary>
        /// Sets up root motion for this actor.
        /// </summary>
        public void SetUpRootMotion(
            bool updateRootPosition = true,
            RootMotionVelocityType rootMotionVelocityType = RootMotionVelocityType.SetVelocity,
            bool updateRootRotation = true,
            RootMotionRotationType rootMotionRotationType = RootMotionRotationType.AddRotation
        )
        {
            UseRootMotion = true;
            UpdateRootPosition = updateRootPosition;
            this.rootMotionVelocityType = rootMotionVelocityType;
            UpdateRootRotation = updateRootRotation;
            this.rootMotionRotationType = rootMotionRotationType;

        }


        /// <summary>
        /// Gets the CharacterBody component associated with this character actor.
        /// </summary>
        public bool Is2D => RigidbodyComponent.Is2D;

        /// <summary>
        /// Gets the RigidbodyComponent component associated with the character.
        /// </summary>
        public override RigidbodyComponent RigidbodyComponent => CharacterBody.RigidbodyComponent;

        /// <summary>
        /// Gets the ColliderComponent component associated with the character.
        /// </summary>
        public ColliderComponent ColliderComponent => CharacterBody.ColliderComponent;

        /// <summary>
        /// Gets the physics component from the character.
        /// </summary>
        public PhysicsComponent PhysicsComponent { get; private set; }

        /// <summary>
        /// Gets the CharacterBody component associated with this character actor.
        /// </summary>
        public CharacterBody CharacterBody { get; private set; }

        /// <summary>
        /// Returns the current character actor state. This enum variable contains the information about the grounded and stable state, all in one.
        /// </summary>
        public CharacterActorState CurrentState
        {
            get
            {
                if (IsGrounded)
                    return IsStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
                else
                    return CharacterActorState.NotGrounded;
            }
        }

        /// <summary>
        /// Returns the character actor state from the previous frame.
        /// </summary>
        public CharacterActorState PreviousState
        {
            get
            {
                if (WasGrounded)
                    return WasStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
                else
                    return CharacterActorState.NotGrounded;
            }
        }

        #region Collision Properties

        public LayerMask ObstaclesLayerMask => PhysicsComponent.CollisionLayerMask | oneWayPlatformsLayerMask;
        public LayerMask ObstaclesWithoutOWPLayerMask => PhysicsComponent.CollisionLayerMask & ~(oneWayPlatformsLayerMask);

        /// <summary>
        /// Returns true if the character is standing on an edge.
        /// </summary>
        public bool IsOnEdge => characterCollisionInfo.isOnEdge;

        /// <summary>
        /// Returns the angle between the both sides of the edge.
        /// </summary>
        public float EdgeAngle => characterCollisionInfo.edgeAngle;

        /// <summary>
        /// Gets the grounded state, true if the ground object is not null, false otherwise.
        /// </summary>
        public bool IsGrounded => characterCollisionInfo.groundObject != null;

        /// <summary>
        /// Gets the angle between the up vector and the stable normal.
        /// </summary>
        public float GroundSlopeAngle => characterCollisionInfo.groundSlopeAngle;

        /// <summary>
        /// Gets the contact point obtained directly from the ground test (sphere cast).
        /// </summary>
        public Vector3 GroundContactPoint => characterCollisionInfo.groundContactPoint;

        /// <summary>
        /// Gets the normal vector obtained directly from the ground test (sphere cast).
        /// </summary>
        public Vector3 GroundContactNormal => characterCollisionInfo.groundContactNormal;

        /// <summary>
        /// Gets the normal vector used to determine stability. This may or may not be the normal obtained from the ground test.
        /// </summary>
        public Vector3 GroundStableNormal => IsStable ? characterCollisionInfo.groundStableNormal : Up;


        /// <summary>
        /// Gets the GameObject component of the current ground.
        /// </summary>
        public GameObject GroundObject => characterCollisionInfo.groundObject;

        /// <summary>
        /// Gets the Transform component of the current ground.
        /// </summary>
        public Transform GroundTransform => GroundObject != null ? GroundObject.transform : null;

        /// <summary>
        /// Gets the Collider2D component of the current ground.
        /// </summary>
        public Collider2D GroundCollider2D => characterCollisionInfo.groundCollider2D;
        /// <summary>
        /// Gets the Collider3D component of the current ground.
        /// </summary>
        public Collider GroundCollider3D => characterCollisionInfo.groundCollider3D;

        /// <summary>
        /// Gets the Rigidbody2D component of the current ground.
        /// </summary>
        public Rigidbody2D GroundRigidbody2D => characterCollisionInfo.groundRigidbody2D;

        /// <summary>
        /// Gets the Rigidbody component of the current ground.
        /// </summary>
        public Rigidbody GroundRigidbody3D => characterCollisionInfo.groundRigidbody3D;

        // Wall ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────	

        /// <summary>
        /// Gets the wall collision flag, true if the character hit a wall, false otherwise.
        /// </summary>
        public bool WallCollision => characterCollisionInfo.wallCollision;


        /// <summary>
        /// Gets the angle between the contact normal (wall collision) and the Up direction.
        /// </summary>	
        public float WallAngle => characterCollisionInfo.wallAngle;


        /// <summary>
        /// Gets the current contact (wall collision).
        /// </summary>
        public Contact WallContact => characterCollisionInfo.wallContact;


        // Head ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────	

        /// <summary>
        /// Gets the head collision flag, true if the character hits something with its head, false otherwise.
        /// </summary>
        public bool HeadCollision => characterCollisionInfo.headCollision;


        /// <summary>
        /// Gets the angle between the contact normal (head collision) and the Up direction.
        /// </summary>
        public float HeadAngle => characterCollisionInfo.headAngle;


        /// <summary>
        /// Gets the current contact (head collision).
        /// </summary>
        public Contact HeadContact => characterCollisionInfo.headContact;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets the current stability state of the character. Stability is equal to "grounded + slope angle <= slope limit".
        /// </summary>
        public bool IsStable
        {
            get
            {
                if (!IsGrounded)
                    return false;

                if (!IsStableLayer(characterCollisionInfo.groundLayer))
                    return false;

                if (WasStable)
                {
                    // If the character is stable, then use the groundSlopeAngle (updated in ProbeGround).
                    return characterCollisionInfo.groundSlopeAngle <= slopeLimit;
                }
                else
                {
                    if (useStableEdgeWhenLanding)
                    {
                        return characterCollisionInfo.groundSlopeAngle <= slopeLimit;
                    }
                    else
                    {
                        // If the character was not stable, then define stability by using the contact normal, instead of the "stable" normal.
                        float contactSlopeAngle = Vector3.Angle(Up, characterCollisionInfo.groundContactNormal);
                        return contactSlopeAngle <= slopeLimit;
                    }

                }
            }
        }


        /// <summary>
        /// Returns true if the character is grounded onto an unstable ground, false otherwise.
        /// </summary>
        public bool IsOnUnstableGround => IsGrounded && characterCollisionInfo.groundSlopeAngle > slopeLimit;

        /// <summary>
        /// Gets the previous grounded state.
        /// </summary>
        public bool WasGrounded { get; private set; }

        /// <summary>
        /// Gets the previous stability state.
        /// </summary>
        public bool WasStable { get; private set; }



        /// <summary>
        /// Gets the RigidbodyComponent component from the ground.
        /// </summary>
        public RigidbodyComponent GroundRigidbodyComponent
        {
            get
            {
                if (!IsStable)
                    groundRigidbodyComponent = null;

                return groundRigidbodyComponent;
            }
        }

        RigidbodyComponent groundRigidbodyComponent = null;

        /// <summary>
        /// Gets the ground rigidbody position.
        /// </summary>
        public Vector3 GroundPosition
        {
            get
            {
                return Is2D ?
                new Vector3(
                    GroundRigidbody2D.position.x,
                    GroundRigidbody2D.position.y,
                    GroundTransform.position.z
                 ) : GroundRigidbody3D.position;
            }
        }

        /// <summary>
        /// Gets the ground rigidbody rotation.
        /// </summary>
        public Quaternion GroundRotation
        {
            get
            {
                return Is2D ? Quaternion.Euler(0f, 0f, GroundRigidbody2D.rotation) : GroundRigidbody3D.rotation;
            }
        }

        /// <summary>
        /// Returns true if the current ground is a Rigidbody (2D or 3D), false otherwise.
        /// </summary>
        public bool IsGroundARigidbody
        {
            get
            {
                return Is2D ? characterCollisionInfo.groundRigidbody2D != null : characterCollisionInfo.groundRigidbody3D != null;
            }
        }

        /// <summary>
        /// Returns true if the current ground is a kinematic Rigidbody (2D or 3D), false otherwise.
        /// </summary>
        public bool IsGroundAKinematicRigidbody
        {
            get
            {
                return Is2D ? characterCollisionInfo.groundRigidbody2D.isKinematic : characterCollisionInfo.groundRigidbody3D.isKinematic;
            }
        }

        /// <summary>
        /// Returns the point velocity (Rigidbody API) of the ground at a given position.
        /// </summary>
        public Vector3 GetGroundPointVelocity(Vector3 position)
        {
            return Is2D ? (Vector3)characterCollisionInfo.groundRigidbody2D.GetPointVelocity(position) : characterCollisionInfo.groundRigidbody3D.GetPointVelocity(position);
        }

        /// <summary>
        /// Returns a concatenated string containing all the current collision information.
        /// </summary>
        public override string ToString()
        {
            const string nullString = " ---- ";


            string triggerString = "";

            for (int i = 0; i < Triggers.Count; i++)
            {
                triggerString += " - " + Triggers[i].gameObject.name + "\n";
            }

            return string.Concat(
                "Ground : \n",
                "──────────────────\n",
                "Is Grounded : ", IsGrounded, '\n',
                "Is Stable : ", IsStable, '\n',
                "Slope Angle : ", characterCollisionInfo.groundSlopeAngle, '\n',
                "Is On Edge : ", characterCollisionInfo.isOnEdge, '\n',
                "Edge Angle : ", characterCollisionInfo.edgeAngle, '\n',
                "Object Name : ", characterCollisionInfo.groundObject != null ? characterCollisionInfo.groundObject.name : nullString, '\n',
                "Layer : ", LayerMask.LayerToName(characterCollisionInfo.groundLayer), '\n',
                "Rigidbody Type: ", GroundRigidbodyComponent != null ? GroundRigidbodyComponent.IsKinematic ? "Kinematic" : "Dynamic" : nullString, '\n',
                "Dynamic Ground : ", GroundRigidbodyComponent != null ? "Yes" : "No", "\n\n",
                "Wall : \n",
                "──────────────────\n",
                "Wall Collision : ", characterCollisionInfo.wallCollision, '\n',
                "Wall Angle : ", characterCollisionInfo.wallAngle, "\n\n",
                "Head : \n",
                "──────────────────\n",
                "Head Collision : ", characterCollisionInfo.headCollision, '\n',
                "Head Angle : ", characterCollisionInfo.headAngle, "\n\n",
                "Triggers : \n",
                "──────────────────\n",
                "Current : ", CurrentTrigger.gameObject != null ? CurrentTrigger.gameObject.name : nullString, '\n',
                triggerString
            );
        }

        #endregion

        protected CharacterCollisionInfo characterCollisionInfo = new CharacterCollisionInfo();

        /// <summary>
        /// Gets a structure with all the information regarding character collisions. Most of the character properties (e.g. IsGrounded, IsStable, GroundObject, and so on)
        /// can be obtained from this structure.
        /// </summary>
        public CharacterCollisionInfo CharacterCollisionInfo => characterCollisionInfo;


#if UNITY_TERRAIN_MODULE
        Dictionary<Transform, Terrain> terrains = new Dictionary<Transform, Terrain>();
#endif
        Dictionary<Transform, RigidbodyComponent> groundRigidbodyComponents = new Dictionary<Transform, RigidbodyComponent>();



        public float GroundedTime { get; private set; }
        public float NotGroundedTime { get; private set; }

        public float StableElapsedTime { get; private set; }
        public float UnstableElapsedTime { get; private set; }


        /// <summary>
        /// Gets the current body size (width and height).
        /// </summary>
        public Vector2 BodySize { get; private set; }

        /// <summary>
        /// Gets the current body size (width and height).
        /// </summary>
        public Vector2 DefaultBodySize => CharacterBody.BodySize;


        /// <summary>
        /// Gets/Sets the rigidbody velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return RigidbodyComponent.Velocity;
            }
            set
            {
                RigidbodyComponent.Velocity = value;
            }
        }



        /// <summary>
        /// Gets/Sets the rigidbody velocity projected onto a plane formed by its up direction.
        /// </summary>
        public Vector3 PlanarVelocity
        {
            get
            {
                return Vector3.ProjectOnPlane(Velocity, Up);
            }
            set
            {
                Velocity = Vector3.ProjectOnPlane(value, Up) + VerticalVelocity;
            }
        }



        /// <summary>
        /// Gets/Sets the rigidbody velocity projected onto its up direction.
        /// </summary>
        public Vector3 VerticalVelocity
        {
            get
            {
                return Vector3.Project(Velocity, Up);
            }
            set
            {
                Velocity = PlanarVelocity + Vector3.Project(value, Up);
            }
        }





        /// <summary>
        /// Gets/Sets the rigidbody velocity projected onto a plane formed by its up direction.
        /// </summary>
        public Vector3 StableVelocity
        {
            get
            {
                return CustomUtilities.ProjectOnTangent(Velocity, GroundStableNormal, Up);
            }
            set
            {
                Velocity = CustomUtilities.ProjectOnTangent(value, GroundStableNormal, Up);
            }
        }


        public Vector3 LastGroundedVelocity { get; private set; }



        /// <summary>
        /// Gets/Sets the rigidbody local velocity.
        /// </summary>
        public Vector3 LocalVelocity
        {
            get
            {
                return transform.InverseTransformVectorUnscaled(Velocity);
            }
            set
            {
                Velocity = transform.TransformVectorUnscaled(value);
            }
        }

        /// <summary>
        /// Gets/Sets the rigidbody local planar velocity.
        /// </summary>
        public Vector3 LocalPlanarVelocity
        {
            get
            {
                return transform.InverseTransformVectorUnscaled(PlanarVelocity);
            }
            set
            {
                PlanarVelocity = transform.TransformVectorUnscaled(value);
            }
        }




        /// <summary>
        /// Returns true if the character local vertical velocity is less than zero. 
        /// </summary>
        public bool IsFalling
        {
            get
            {
                return LocalVelocity.y < 0f;
            }
        }

        /// <summary>
        /// Returns true if the character local vertical velocity is greater than zero.
        /// </summary>
        public bool IsAscending
        {
            get
            {
                return LocalVelocity.y > 0f;
            }
        }



        #region public Body properties

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return GetCenter(Position);
            }
        }

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 Top
        {
            get
            {
                return GetTop(Position);
            }
        }

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 Bottom
        {
            get
            {
                return GetBottom(Position);
            }
        }

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 TopCenter
        {
            get
            {
                return GetTopCenter(Position);
            }
        }

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 BottomCenter
        {
            get
            {
                return GetBottomCenter(Position, 0f);
            }
        }

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 OffsettedBottomCenter
        {
            get
            {
                return GetBottomCenter(Position, StepOffset);
            }
        }

        #endregion

        #region Body functions

        /// <summary>
        /// Gets the center of the collision shape.
        /// </summary>
        public Vector3 GetCenter(Vector3 position)
        {
            return position + CustomUtilities.Multiply(Up, BodySize.y / 2f);
        }

        /// <summary>
        /// Gets the top most point of the collision shape.
        /// </summary>
        public Vector3 GetTop(Vector3 position)
        {
            return position + CustomUtilities.Multiply(Up, BodySize.y - CharacterConstants.SkinWidth);
        }

        /// <summary>
        /// Gets the bottom most point of the collision shape.
        /// </summary>
        public Vector3 GetBottom(Vector3 position)
        {
            return position + CustomUtilities.Multiply(Up, CharacterConstants.SkinWidth);
        }

        /// <summary>
        /// Gets the center of the top sphere of the collision shape.
        /// </summary>
        public Vector3 GetTopCenter(Vector3 position)
        {
            return position + CustomUtilities.Multiply(Up, BodySize.y - BodySize.x / 2f);
        }

        /// <summary>
        /// Gets the center of the top sphere of the collision shape (considering an arbitrary body size).
        /// </summary>
        public Vector3 GetTopCenter(Vector3 position, Vector2 bodySize)
        {
            return position + CustomUtilities.Multiply(Up, bodySize.y - bodySize.x / 2f);
        }

        /// <summary>
        /// Gets the center of the bottom sphere of the collision shape.
        /// </summary>
        public Vector3 GetBottomCenter(Vector3 position, float bottomOffset = 0f)
        {
            return position + CustomUtilities.Multiply(Up, BodySize.x / 2f + bottomOffset);
        }


        /// <summary>
        /// Gets the center of the bottom sphere of the collision shape (considering an arbitrary body size).
        /// </summary>
        public Vector3 GetBottomCenter(Vector3 position, Vector2 bodySize)
        {
            return position + CustomUtilities.Multiply(Up, bodySize.x / 2f);
        }

        /// <summary>
        /// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
        /// </summary>
        public Vector3 GetBottomCenterToTopCenter()
        {
            return CustomUtilities.Multiply(Up, BodySize.y - BodySize.x);
        }

        /// <summary>
        /// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
        /// </summary>
        public Vector3 GetBottomCenterToTopCenter(Vector2 bodySize)
        {
            return CustomUtilities.Multiply(Up, bodySize.y - bodySize.x);
        }


        #endregion


        CharacterCollisions characterCollisions = new CharacterCollisions();

        public CharacterCollisions CharacterCollisions => characterCollisions;


        // GameObject groundTriggerObject = null;

        protected override void Awake()
        {

            base.Awake();


            CharacterBody = GetComponent<CharacterBody>();
            targetBodySize = CharacterBody.BodySize;
            BodySize = targetBodySize;

            if (Is2D)
                PhysicsComponent = gameObject.AddComponent<PhysicsComponent2D>();
            else
                PhysicsComponent = gameObject.AddComponent<PhysicsComponent3D>();


            RigidbodyComponent.IsKinematic = false;
            RigidbodyComponent.UseGravity = false;
            RigidbodyComponent.Mass = CharacterBody.Mass;
            RigidbodyComponent.LinearDrag = 0f;
            RigidbodyComponent.AngularDrag = 0f;
            RigidbodyComponent.Constraints = RigidbodyConstraints.FreezeRotation;


            characterCollisions.Initialize(this, PhysicsComponent);

            // Ground trigger
            if (Is2D)
            {
                groundTriggerCollider2D = gameObject.AddComponent<CircleCollider2D>();
                groundTriggerCollider2D.hideFlags = HideFlags.NotEditable;
                groundTriggerCollider2D.isTrigger = true;
                groundTriggerCollider2D.radius = BodySize.x / 2f;
                groundTriggerCollider2D.offset = Vector2.up * (BodySize.x / 2f - CharacterConstants.GroundTriggerOffset);

                Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), groundTriggerCollider2D, true);

            }
            else
            {
                groundTriggerCollider3D = gameObject.AddComponent<SphereCollider>();
                groundTriggerCollider3D.hideFlags = HideFlags.NotEditable;
                groundTriggerCollider3D.isTrigger = true;
                groundTriggerCollider3D.radius = BodySize.x / 2f;
                groundTriggerCollider3D.center = Vector3.up * (BodySize.x / 2f - CharacterConstants.GroundTriggerOffset);

                Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), groundTriggerCollider3D, true);
            }

        }

        CircleCollider2D groundTriggerCollider2D = null;
        SphereCollider groundTriggerCollider3D = null;

        protected override void Start()
        {
            base.Start();

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesLayerMask,
                false,
                true,
                oneWayPlatformsLayerMask
            );

            // Initial OWP check
            characterCollisions.CheckOverlapWithLayerMask(
                Position,
                0f,
                in filter
            );

            // Initial "Force Grounded"
            if (forceGroundedAtStart && !alwaysNotGrounded)
                ForceGrounded();

            SetColliderSize(!IsStable);

            forward2D = transform.right;



        }


        protected override void OnEnable()
        {
            base.OnEnable();

            OnTeleport += OnTeleportMethod;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            OnTeleport -= OnTeleportMethod;
        }


        void OnTeleportMethod(Vector3 position, Quaternion rotation)
        {
            Velocity = Vector3.zero;
        }


        /// <summary>
        /// Applies a force at the ground contact point, in the direction of the weight (mass times gravity).
        /// </summary>
        protected virtual void ApplyWeight(Vector3 contactPoint)
        {
            if (!applyWeightToGround)
                return;


            if (Is2D)
            {
                if (GroundCollider2D == null)
                    return;

                if (GroundCollider2D.attachedRigidbody == null)
                    return;

                GroundCollider2D.attachedRigidbody.AddForceAtPosition(CustomUtilities.Multiply(-Up, CharacterBody.Mass, weightGravity), contactPoint);
            }
            else
            {
                if (GroundCollider3D == null)
                    return;

                if (GroundCollider3D.attachedRigidbody == null)
                    return;


                GroundCollider3D.attachedRigidbody.AddForceAtPosition(CustomUtilities.Multiply(-Up, CharacterBody.Mass, weightGravity), contactPoint);
            }


        }



        // -------------------------------------------------------------------------------------------------------


        void SetColliderSize(bool fullSize)
        {

            float verticalOffset = fullSize ? 0f : Mathf.Max(StepOffset, CharacterConstants.ColliderMinBottomOffset);

            float radius = BodySize.x / 2f;
            float height = BodySize.y - verticalOffset;

            ColliderComponent.Size = new Vector2(2f * radius, height);
            ColliderComponent.Offset = CustomUtilities.Multiply(Vector2.up, verticalOffset + height / 2f);
        }



        /// <summary>
        /// Gets/Sets the current rigidbody position. This action will produce an "interpolation reset", meaning that (visually) the object will move instantly to the target.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return RigidbodyComponent.Position;
            }
            set
            {
                RigidbodyComponent.Position = value;


                ResetInterpolationPosition();
            }
        }

        /// <summary>
        /// Gets/Sets the current rigidbody rotation. This action will produce an "interpolation reset", meaning that (visually) the object will rotate instantly to the target.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return RigidbodyComponent.Rotation;
            }
            set
            {
                RigidbodyComponent.Rotation = value;

                ResetInterpolationRotation();
            }
        }

        /// <summary>
        /// Gets/Sets the current up direction based on the rigidbody rotation (not necessarily transform.up).
        /// </summary>
        public Vector3 Up
        {
            get
            {
                return RigidbodyComponent.Up;
            }
            set
            {
                if (value == Vector3.zero)
                    return;

                value.Normalize();
                Quaternion deltaRotation = Quaternion.FromToRotation(Up, value);

                RotateInternal(deltaRotation);
            }
        }

        Vector3 forward2D = Vector3.right;

        /// <summary>
        /// Gets/Sets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                return Is2D ? forward2D : RigidbodyComponent.Rotation * Vector3.forward;
            }
            set
            {

                if (value == Vector3.zero)
                    return;

                if (Is2D)
                {
                    forward2D = Vector3.Project(value, Right);
                    forward2D.Normalize();
                }
                else
                {
                    // If the up direction is fixed, then make sure the rotation is 100% yaw (up axis).
                    if (constraintRotation)
                    {
                        float signedAngle = Vector3.SignedAngle(Forward, value, Up);
                        Quaternion deltaRotation = Quaternion.AngleAxis(signedAngle, Up);
                        RotateInternal(deltaRotation);
                    }
                    else
                    {
                        Quaternion deltaRotation = Quaternion.FromToRotation(Forward, value);
                        RotateInternal(deltaRotation);
                    }



                }


            }
        }

        /// <summary>
        /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right).
        /// </summary>
        public Vector3 Right
        {
            get
            {
                return RigidbodyComponent.Rotation * Vector3.right;
            }
        }

        /// <summary>
        /// Rotates the character doing yaw rotation (around its vertical axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        public void SetYaw(float angle)
        {
            Forward = Quaternion.AngleAxis(angle, Up) * Forward;
        }


        // Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets a list with all the current contacts.
        /// </summary>
        public List<Contact> Contacts
        {
            get
            {
                if (PhysicsComponent == null)
                    return null;

                return PhysicsComponent.Contacts;
            }
        }



        // Triggers ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets the most recent trigger.
        /// </summary>
        public Trigger CurrentTrigger
        {
            get
            {
                if (PhysicsComponent.Triggers.Count == 0)
                    return new Trigger();   // "Null trigger"

                return PhysicsComponent.Triggers[PhysicsComponent.Triggers.Count - 1];
            }
        }

        /// <summary>
        /// Gets a list with all the triggers.
        /// </summary>
        public List<Trigger> Triggers
        {
            get
            {
                return PhysicsComponent.Triggers;
            }
        }


        public bool IsKinematic
        {
            get
            {
                return RigidbodyComponent.IsKinematic;
            }
            set
            {
                RigidbodyComponent.IsKinematic = value;
            }
        }

        public enum CharacterVelocityMode
        {
            UseInputVelocity,
            UsePreSimulationVelocity,
            UsePostSimulationVelocity
        }


        /// <summary>
        /// Gets the character velocity vector (Velocity) assigned prior to the FixedUpdate call. This is also known as the "input" velocity, 
        /// since it is the value the user has specified.
        /// </summary>
        public Vector3 InputVelocity { get; private set; }

        /// <summary>
        /// Gets a velocity vector which is the input velocity modified, based on the character actor internal rules (step up, slope limit, etc). 
        /// This velocity corresponds to the one used by the physics simulation.
        /// </summary>
        public Vector3 PreSimulationVelocity { get; private set; }

        /// <summary>
        /// Gets the character velocity as the result of the Physics simulation.
        /// </summary>
        public Vector3 PostSimulationVelocity { get; private set; }

        /// <summary>
        /// Gets the difference between the post-simulation velocity (after the physics simulation) and the pre-simulation velocity (just before the physics simulation). 
        /// This value is useful to detect any external response due to the physics simulation, such as hits coming from other rigidbodies.
        /// </summary>
        public Vector3 ExternalVelocity { get; private set; }


        void HandleRotation(float dt)
        {

            if (!constraintRotation)
                return;

            if (upDirectionReference != null)
            {
                Vector3 targetPosition = Position + CustomUtilities.Multiply(Velocity, dt);
                float sign = upDirectionReferenceMode == VerticalAlignmentSettings.VerticalReferenceMode.Towards ? 1f : -1f;

                constraintUpDirection = CustomUtilities.Multiply(Vector3.Normalize(upDirectionReference.position - targetPosition), sign);

            }

            Up = constraintUpDirection;

        }


        Vector2 targetBodySize;

        public enum SizeReferenceType
        {
            Top,
            Center,
            Bottom
        }



        void HandleSize(float dt)
        {
            Vector2 previousBodySize = BodySize;
            BodySize = Vector2.Lerp(BodySize, targetBodySize, sizeLerpSpeed * dt);

            SetColliderSize(!IsStable);

            if (!IsGrounded)
            {
                float verticalOffset = 0f;

                switch (sizeReferenceType)
                {
                    case SizeReferenceType.Top:
                        verticalOffset = Mathf.Abs(previousBodySize.y - BodySize.y);
                        break;
                    case SizeReferenceType.Center:
                        verticalOffset = Mathf.Abs(previousBodySize.y - BodySize.y) / 2f;
                        break;
                    case SizeReferenceType.Bottom:
                        verticalOffset = 0f;
                        break;
                }


                if (previousBodySize.y != BodySize.y)
                    RigidbodyComponent.Position += CustomUtilities.Multiply(Vector3.up, verticalOffset);

            }



        }

        List<Contact> wallContacts = new List<Contact>(10);

        /// <summary>
        /// Returns a lits of all the contacts involved with wall collision events.
        /// </summary>
        public List<Contact> WallContacts => wallContacts;


        List<Contact> headContacts = new List<Contact>(10);

        /// <summary>
        /// Returns a lits of all the contacts involved with head collision events.
        /// </summary>
        public List<Contact> HeadContacts => headContacts;

        List<Contact> groundContacts = new List<Contact>(10);

        /// <summary>
        /// Returns a lits of all the contacts involved with head collision events.
        /// </summary>
        public List<Contact> GroundContacts => groundContacts;


        float unstableGroundContactTime = 0f;

        void GetContactsInformation()
        {
            // bool groundRigidbodyHitFlag = false;
            bool wasCollidingWithWall = characterCollisionInfo.wallCollision;
            bool wasCollidingWithHead = characterCollisionInfo.headCollision;

            groundContacts.Clear();
            wallContacts.Clear();
            headContacts.Clear();

            for (int i = 0; i < Contacts.Count; i++)
            {
                Contact contact = Contacts[i];

                float verticalAngle = Vector3.Angle(Up, contact.normal);

                // Get the wall collision information -------------------------------------------------------------			
                if (CustomUtilities.isCloseTo(verticalAngle, 90f, CharacterConstants.WallContactAngleTolerance))
                    wallContacts.Add(contact);


                // Get the head collision information -----------------------------------------------------------------
                if (verticalAngle >= CharacterConstants.HeadContactMinAngle)
                    headContacts.Add(contact);

                if (verticalAngle <= 89f)
                    groundContacts.Add(contact);


            }


            if (wallContacts.Count == 0)
            {
                characterCollisionInfo.ResetWallInfo();
            }
            else
            {
                Contact wallContact = wallContacts[0];

                characterCollisionInfo.SetWallInfo(in wallContact, this);

                if (!wasCollidingWithWall)
                {
                    if (OnWallHit != null)
                        OnWallHit(wallContact);
                }

            }


            if (headContacts.Count == 0)
            {
                characterCollisionInfo.ResetHeadInfo();
            }
            else
            {
                Contact headContact = headContacts[0];

                characterCollisionInfo.SetHeadInfo(in headContact, this);

                if (!wasCollidingWithHead)
                {
                    if (OnHeadHit != null)
                        OnHeadHit(headContact);

                }
            }


        }

        Vector3 preSimulationPosition = default(Vector3);



        protected override void PreSimulationUpdate(float dt)
        {
            PhysicsComponent.ClearContacts();

            if (!forceNotGroundedFlag)
            {
                WasGrounded = IsGrounded;
                WasStable = IsStable;
            }

            InputVelocity = Velocity;

            HandleSize(dt);
            HandlePosition(dt);

            PreSimulationVelocity = Velocity;
            preSimulationPosition = Position;

            // ------------------------------------------------------------

            if (IsStable)
            {
                StableElapsedTime += dt;
                UnstableElapsedTime = 0f;
            }
            else
            {
                StableElapsedTime = 0f;
                UnstableElapsedTime += dt;
            }

            if (IsGrounded)
            {
                NotGroundedTime = 0f;
                GroundedTime += dt;

                LastGroundedVelocity = Velocity;

                if (!WasGrounded)
                    if (OnGroundedStateEnter != null)
                        OnGroundedStateEnter(LocalVelocity);

            }
            else
            {
                NotGroundedTime += dt;
                GroundedTime = 0f;

                if (WasGrounded)
                    if (OnGroundedStateExit != null)
                        OnGroundedStateExit();

            }

            if (forceNotGroundedFrames != 0)
                forceNotGroundedFrames--;



            // Enable/Disable the ground trigger.
            if (Is2D)
                groundTriggerCollider2D.enabled = useGroundTrigger;
            else
                groundTriggerCollider3D.enabled = useGroundTrigger;

            forceNotGroundedFlag = false;
        }



        protected override void PostSimulationUpdate(float dt)
        {
            HandleRotation(dt);

            GetContactsInformation();

            PostSimulationVelocity = Velocity;
            ExternalVelocity = PostSimulationVelocity - PreSimulationVelocity;


            if (IsStable && !IsKinematic)
            {
                ProcessDynamicGroundMovement(dt);


                PreGroundProbingPosition = Position;

                ProbeGround(dt);

                PostGroundProbingPosition = Position;
                GroundProbingDisplacement = Position - PreGroundProbingPosition;
            }
            else
            {
                GroundProbingDisplacement = Vector3.zero;
            }

            // Velocity assignment ------------------------------------------------------
            SetPostSimulationVelocity();
        }

        Vector3 groundToCharacter;

        void UpdateDynamicGround(ref Vector3 position, ref Quaternion rotation, float dt)
        {
            Quaternion deltaRotation = GroundRotation * Quaternion.Inverse(preSimulationGroundRotation);

            Vector3 localGroundToCharacter = GroundTransform.InverseTransformVectorUnscaled(groundToCharacter);
            Vector3 rotatedGroundToCharacter = GroundTransform.rotation * localGroundToCharacter;

            position = GroundPosition + (deltaRotation * groundToCharacter);


            if (!Is2D && rotateForwardDirection)
            {
                // Quaternion deltaRotation = referenceRigidbodyRotation * Quaternion.Inverse( GroundTransform.rotation );
                Vector3 forward = deltaRotation * Forward;
                forward = Vector3.ProjectOnPlane(forward, Up);
                forward.Normalize();

                rotation = Quaternion.LookRotation(forward, Up);
            }


            PreviousGroundVelocity = GroundVelocity;
            GroundVelocity = (position - Position) / dt;

        }

        void ProcessInheritedVelocity()
        {
            if (!forceNotGroundedFlag)
                return;

            // "local" to the character
            Vector3 localGroundVelocity = transform.InverseTransformVectorUnscaled(GroundVelocity);
            Vector3 planarGroundVelocity = Vector3.ProjectOnPlane(GroundVelocity, Up);
            Vector3 verticalGroundVelocity = Vector3.Project(GroundVelocity, Up);

            Vector3 inheritedGroundVelocity = Vector3.zero;

            if (planarGroundVelocity.magnitude >= inheritedGroundPlanarVelocityThreshold)
                inheritedGroundVelocity += CustomUtilities.Multiply(planarGroundVelocity, inheritedGroundPlanarVelocityMultiplier);

            if (verticalGroundVelocity.magnitude >= inheritedGroundVerticalVelocityThreshold)
            {

                // This prevents an edge case where the character is unable to jump (descending platform)
                if (LocalVelocity.y > -localGroundVelocity.y)
                    inheritedGroundVelocity += CustomUtilities.Multiply(verticalGroundVelocity, inheritedGroundVerticalVelocityMultiplier);
            }


            Velocity += inheritedGroundVelocity;

            GroundVelocity = Vector3.zero;
            PreviousGroundVelocity = Vector3.zero;

        }

        void ProcessDynamicGroundMovement(float dt)
        {
            if (!supportDynamicGround || !IsGroundARigidbody || !CustomUtilities.BelongsToLayerMask(GroundObject.layer, dynamicGroundLayerMask))
                return;

            // The ground might hit the character really hard (e.g. fast ascending platform), causing this to get some extra velocity (unwanted behaviour). 
            // So, ignore any incoming velocity from the ground by replacing it with the input velocity.
            IgnoreGroundCollision();


            Vector3 targetPosition = Position;
            Quaternion targetRotation = Rotation;

            UpdateDynamicGround(ref targetPosition, ref targetRotation, dt);


            if (!IsGroundAOneWayPlatform && GroundDeltaVelocity.magnitude > maxGroundVelocityChange)
            {
                float upToDynamicGroundVelocityAngle = Vector3.Angle(Vector3.Normalize(GroundVelocity), Up);


                if (upToDynamicGroundVelocityAngle < 45f)
                    ForceNotGrounded();


                Vector3 characterVelocity = PreviousGroundVelocity;

                RigidbodyComponent.Velocity = characterVelocity;
                RigidbodyComponent.Position += CustomUtilities.Multiply(characterVelocity, dt);
                RigidbodyComponent.Rotation = targetRotation;

            }
            else
            {
                Vector3 position = Position;
                PostSimulationCollideAndSlide(ref position, ref targetRotation, targetPosition - position, false);
                RigidbodyComponent.Position = position;
                RigidbodyComponent.Rotation = targetRotation;

            }

        }


        void SetPostSimulationVelocity()
        {
            if (IsStable)
            {

                switch (stablePostSimulationVelocity)
                {
                    case CharacterVelocityMode.UseInputVelocity:

                        Velocity = InputVelocity;

                        break;
                    case CharacterVelocityMode.UsePreSimulationVelocity:

                        Velocity = PreSimulationVelocity;

                        // Take the rigidbody velocity and convert it into planar velocity
                        if (WasStable)
                            PlanarVelocity = CustomUtilities.Multiply(Vector3.Normalize(PlanarVelocity), Velocity.magnitude);

                        break;
                    case CharacterVelocityMode.UsePostSimulationVelocity:

                        // Take the rigidbody velocity and convert it into planar velocity
                        if (WasStable)
                            PlanarVelocity = CustomUtilities.Multiply(Vector3.Normalize(PlanarVelocity), Velocity.magnitude);

                        break;
                }


            }
            else
            {
                switch (unstablePostSimulationVelocity)
                {
                    case CharacterVelocityMode.UseInputVelocity:

                        Velocity = InputVelocity;

                        break;
                    case CharacterVelocityMode.UsePreSimulationVelocity:

                        Velocity = PreSimulationVelocity;

                        break;
                    case CharacterVelocityMode.UsePostSimulationVelocity:

                        break;
                }
            }
        }



        void IgnoreGroundCollision()
        {
            for (int i = 0; i < Contacts.Count; i++)
            {
                if (!Contacts[i].isRigidbody)
                    continue;

                if (!Contacts[i].isKinematicRigidbody)
                    continue;

                if (Contacts[i].gameObject.transform == GroundTransform)
                {
                    Velocity = InputVelocity;
                    break;
                }
            }
        }


        #region Events



        /// <summary>
        /// This event is called when the character hits its head (not grounded).
        /// 
        /// The related collision information struct is passed as an argument.
        /// </summary>
        public event System.Action<Contact> OnHeadHit;

        /// <summary>
        /// This event is called everytime the character is blocked by an unallowed geometry, this could be
        /// a wall or a steep slope (depending on the "slopeLimit" value).
        /// 
        /// The related collision information struct is passed as an argument.
        /// </summary>
        public event System.Action<Contact> OnWallHit;

        /// <summary>
        /// This event is called everytime the character teleports.
        /// 
        /// The teleported position and rotation are passed as arguments.
        /// </summary>
        public event System.Action<Vector3, Quaternion> OnTeleport;

        /// <summary>
        /// This event is called when the character enters the grounded state.
        /// 
        /// The local linear velocity is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnGroundedStateEnter;

        /// <summary>
        /// This event is called when the character exits the grounded state.
        /// </summary>
        public event System.Action OnGroundedStateExit;

        /// <summary>
        /// This event is called when the character make contact with a new ground (object).
        /// </summary>
        public event System.Action OnNewGroundEnter;

        #endregion


        /// <summary>
        /// Sets the teleportation position and rotation using an external Transform reference. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Transform reference)
        {
            Teleport(reference.position, reference.rotation);

        }

        /// <summary>
        /// Sets the teleportation position and rotation. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;

            if (OnTeleport != null)
                OnTeleport(Position, Rotation);
        }

        /// <summary>
        /// Sets the teleportation position. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            Position = position;

            if (OnTeleport != null)
                OnTeleport(Position, Rotation);

        }


        /// <summary>
        /// Gets the velocity of the ground (rigidbody).
        /// </summary>
        public Vector3 GroundVelocity { get; private set; }

        /// <summary>
        /// Gets the previous velocity of the ground (rigidbody).
        /// </summary>
        public Vector3 PreviousGroundVelocity { get; private set; }

        /// <summary>
        /// The ground change in velocity (current velocity - previous velocity).
        /// </summary>
        public Vector3 GroundDeltaVelocity => GroundVelocity - PreviousGroundVelocity;

        /// <summary>
        /// The ground acceleration (GroundDeltaVelocity / dt).
        /// </summary>
        public Vector3 GroundAcceleration => (GroundVelocity - PreviousGroundVelocity) / Time.fixedDeltaTime;


        /// <summary>
        /// Returns true if the ground vertical displacement (moving ground) is positive.
        /// </summary>
        public bool IsGroundAscending => transform.InverseTransformVectorUnscaled(Vector3.Project(CustomUtilities.Multiply(GroundVelocity, Time.deltaTime), Up)).y > 0;

#if UNITY_TERRAIN_MODULE

        /// <summary>
        /// Gets the current terrain the character is standing on.
        /// </summary>
        public Terrain CurrentTerrain { get; private set; }

        /// <summary>
        /// Returns true if the character is standing on a terrain.
        /// </summary>
        public bool IsOnTerrain => CurrentTerrain != null;

#endif

        bool IsAllowedToFollowRigidbodyReference => IsStable && supportDynamicGround && IsGroundARigidbody && CustomUtilities.BelongsToLayerMask(GroundObject.layer, dynamicGroundLayerMask);


        void SetDynamicGroundData(Vector3 position)
        {

            if (IsAllowedToFollowRigidbodyReference)
            {
                preSimulationGroundPosition = GroundPosition;
                preSimulationGroundRotation = GroundRotation;
                groundToCharacter = position - GroundPosition;

                GroundVelocity = PreviousGroundVelocity = GetGroundPointVelocity(position);

            }
            else
            {
                GroundVelocity = Vector3.zero;
                PreviousGroundVelocity = Vector3.zero;
            }

        }



        void HandlePosition(float dt)
        {

            Vector3 position = Position;

            if (alwaysNotGrounded)
                ForceNotGrounded();

            if (IsKinematic)
                return;



            if (IsStable)
            {
                ApplyWeight(GroundContactPoint);

                VerticalVelocity = Vector3.zero;

                Vector3 displacement = CustomUtilities.ProjectOnTangent(
                    CustomUtilities.Multiply(Velocity, dt),
                    GroundStableNormal,
                    Up
                );

                StableCollideAndSlide(ref position, displacement, false);

                SetDynamicGroundData(position);

                if (!IsStable)
                {

#if UNITY_TERRAIN_MODULE
                    CurrentTerrain = null;
#endif
                    groundRigidbodyComponent = null;
                }

            }
            else
            {

                ProcessInheritedVelocity();

                Vector3 displacement = CustomUtilities.Multiply(Velocity, dt);
                UnstableCollideAndSlide(ref position, displacement, dt);

                SetDynamicGroundData(position);

            }

            Move(position);
        }


        Vector3 preSimulationGroundPosition;
        Quaternion preSimulationGroundRotation;



        /// <summary>
        /// Sets the rigidbody velocity based on a target position. The same can be achieved by setting the velocity value manually.
        /// </summary>
        public void Move(Vector3 position)
        {
            RigidbodyComponent.Move(position);
        }




        protected override void UpdateDynamicRootMotionPosition(Vector3 deltaPosition)
        {
            Vector3 rootMotionVelocity = deltaPosition / Time.deltaTime;

            switch (rootMotionVelocityType)
            {
                case RootMotionVelocityType.SetVelocity:
                    Velocity = rootMotionVelocity;
                    break;
                case RootMotionVelocityType.SetPlanarVelocity:
                    PlanarVelocity = rootMotionVelocity;
                    break;
                case RootMotionVelocityType.SetVerticalVelocity:
                    VerticalVelocity = rootMotionVelocity;
                    break;
            }
        }


        void RotateInternal(Quaternion deltaRotation)
        {

            Vector3 preRotationCenter = IsGrounded ? GetBottomCenter(Position) : GetCenter(Position);

            RigidbodyComponent.Rotation = deltaRotation * RigidbodyComponent.Rotation;

            Vector3 postRotationCenter = IsGrounded ? GetBottomCenter(Position) : GetCenter(Position);

            RigidbodyComponent.Position += preRotationCenter - postRotationCenter;
        }







        /// <summary>
        /// Checks if the new character size fits in place. If this check is valid then the real size of the character is changed.
        /// </summary>
        public bool SetBodySize(Vector2 size)
        {

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesWithoutOWPLayerMask,
                true,
                true
            );

            if (!characterCollisions.CheckBodySize(size, Position, in filter))
                return false;

            targetBodySize = size;

            return true;
        }

        /// <summary>
        /// Sweeps the body from its current position (CharacterActor.Position) towards the desired destination using the "collide and slide" algorithm. 
        /// At the end, the character will be moved to a valid position. Triggers and one way platforms will be ignored.
        /// </summary>
        public void SweepAndTeleport(Vector3 destination)
        {
            HitInfoFilter filter = new HitInfoFilter(ObstaclesWithoutOWPLayerMask, false, true);
            SweepAndTeleport(destination, in filter);
        }

        /// <summary>
        /// Sweeps the body from its current position (CharacterActor.Position) towards the desired destination using the "collide and slide" algorithm. 
        /// At the end, the character will be moved to a valid position. 
        /// </summary>
        public void SweepAndTeleport(Vector3 destination, in HitInfoFilter filter)
        {
            Vector3 displacement = destination - Position;
            CollisionInfo collisionInfo = characterCollisions.CastBody(
                Position,
                displacement,
                0f,
                in filter
            );

            Position += collisionInfo.displacement;
        }

        /// <summary>
        /// Forces the character to be grounded (isGrounded = true) if possible. The detection distance includes the step down distance.
        /// </summary>
        public void ForceGrounded()
        {
            if (!CanEnterGroundedState)
                return;

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesLayerMask,
                false,
                true,
                oneWayPlatformsLayerMask
            );

            CollisionInfo collisionInfo = characterCollisions.CheckForGround(
                Position,
                BodySize.y * 0.8f, // 80% of the height
                stepDownDistance,
                filter
            );


            if (collisionInfo.hitInfo.hit)
            {
                ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);

                float slopeAngle = Vector3.Angle(Up, GetGroundSlopeNormal(collisionInfo));

                if (slopeAngle <= slopeLimit)
                {
                    // Save the ground collision info
                    characterCollisionInfo.SetGroundInfo(collisionInfo, this, true);
                    Position += collisionInfo.displacement;

                    SetDynamicGroundData(Position);
                }



            }
        }


        public Vector3 GetGroundSlopeNormal(in CollisionInfo collisionInfo)
        {

#if UNITY_TERRAIN_MODULE
            if (IsOnTerrain)
                return collisionInfo.hitInfo.normal;
#endif

            float contactSlopeAngle = Vector3.Angle(Up, collisionInfo.hitInfo.normal);
            if (collisionInfo.isAnEdge)
            {
                if (contactSlopeAngle < slopeLimit && collisionInfo.edgeUpperSlopeAngle <= slopeLimit && collisionInfo.edgeLowerSlopeAngle <= slopeLimit)
                {
                    return Up;
                }
                else if (collisionInfo.edgeUpperSlopeAngle <= slopeLimit)
                {
                    return collisionInfo.edgeUpperNormal;
                }
                else if (collisionInfo.edgeLowerSlopeAngle <= slopeLimit)
                {
                    return collisionInfo.edgeLowerNormal;
                }
                else
                {
                    return collisionInfo.hitInfo.normal;
                }
            }
            else
            {
                return collisionInfo.hitInfo.normal;
            }



        }

        /// <summary>
        /// The last vertical displacement calculated by the ground probabing algorithm (PostGroundProbingPosition - PreGroundProbingPosition).
        /// </summary>
        public Vector3 GroundProbingDisplacement { get; private set; }

        /// <summary>
        /// The last rigidbody position prior to the ground probing algorithm.
        /// </summary>
        public Vector3 PreGroundProbingPosition { get; private set; }

        /// <summary>
        /// The last rigidbody position after the ground probing algorithm.
        /// </summary>
        public Vector3 PostGroundProbingPosition { get; private set; }

        bool IsStableLayer(int layer)
        {
            return CustomUtilities.BelongsToLayerMask(layer, stableLayerMask);
        }


        void ProbeGround(float dt)
        {
            Vector3 position = Position;

            float groundCheckDistance = edgeCompensation ?
            BodySize.x / 2f + CharacterConstants.GroundCheckDistance :
            CharacterConstants.GroundCheckDistance;

            Vector3 displacement = CustomUtilities.Multiply(-Up, Mathf.Max(groundCheckDistance, stepDownDistance));

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesLayerMask,
                false,
                true
            );

            CollisionInfo collisionInfo = characterCollisions.CheckForGround(
                position,
                StepOffset,
                stepDownDistance,
                in filter
            );

            if (collisionInfo.hitInfo.hit)
            {
                float slopeAngle = Vector3.Angle(Up, GetGroundSlopeNormal(in collisionInfo));

                if (slopeAngle <= slopeLimit && IsStableLayer(collisionInfo.hitInfo.layer))
                {
                    // Stable hit ---------------------------------------------------				
                    ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);

                    // Save the ground collision info
                    characterCollisionInfo.SetGroundInfo(collisionInfo, this, true);

                    position += collisionInfo.displacement;

                    if (edgeCompensation && IsAStableEdge(in collisionInfo))
                    {
                        // calculate the edge compensation and apply that to the final position
                        Vector3 compensation = Vector3.Project((collisionInfo.hitInfo.point - position), Up);
                        position += compensation;
                    }


                }
                else
                {

                    if (preventBadSteps)
                    {

                        if (WasGrounded)
                        {
                            // Restore the initial position and simulate again.
                            Vector3 dynamicGroundDisplacement = CustomUtilities.Multiply(GroundVelocity, dt);
                            Vector3 initialPosition = preSimulationPosition + dynamicGroundDisplacement;
                            position = initialPosition;

                            Vector3 unstableDisplacement = CustomUtilities.ProjectOnTangent(
                                CustomUtilities.Multiply(InputVelocity, dt),
                                GroundStableNormal,
                                Up
                            );
                            // Vector3 unstableDisplacement = Vector3.ProjectOnPlane( CustomUtilities.Multiply( InputVelocity , dt ) , GroundStableNormal );

                            StableCollideAndSlide(ref position, unstableDisplacement, true);

                            // If the body is 2D then redefine velocity.
                            // This eliminates a small "velocity leak" caused by the collide and slide algorithm in 2D.
                            if (Is2D)
                                Velocity = (position - initialPosition) / dt;
                        }
                    }

                    // Re-use the old collisionInfo reference
                    collisionInfo = characterCollisions.CheckForGroundRay(
                        position,
                        filter
                    );

                    ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);

                    characterCollisionInfo.SetGroundInfo(collisionInfo, this);

                }



            }
            else
            {
                ForceNotGrounded();

            }

            if (IsStable)
            {
                RigidbodyComponent.Position = position;
            }

        }


        int forceNotGroundedFrames = 0;

        /// <summary>
        /// Forces the character to abandon the grounded state (isGrounded = false). 
        /// 
        /// TIP: This is useful when making the character jump.
        /// </summary>
        /// <param name="ignoreGroundContactFrames">The number of FixedUpdate frames to consume in order to prevent the character to 
        /// re-enter grounded state right after a ForceNotGrounded call.</param>
        public void ForceNotGrounded(int ignoreGroundContactFrames = 3)
        {
            forceNotGroundedFrames = ignoreGroundContactFrames;

            WasGrounded = IsGrounded;
            WasStable = IsStable;

            characterCollisionInfo.ResetGroundInfo();

            forceNotGroundedFlag = true;
        }

        bool forceNotGroundedFlag = false;

        bool IsAStableEdge(in CollisionInfo collisionInfo)
        {
            return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle <= slopeLimit;
        }

        bool IsAnUnstableEdge(in CollisionInfo collisionInfo)
        {
            return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle > slopeLimit;
        }

        protected void StableCollideAndSlide(ref Vector3 position, Vector3 displacement, bool useFullBody)
        {

            Vector3 groundPlaneNormal = GroundStableNormal;
            Vector3 slidingPlaneNormal = Vector3.zero;

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesLayerMask,
                false,
                true,
                oneWayPlatformsLayerMask
            );

            int iteration = 0;


            while (iteration < CharacterConstants.MaxSlideIterations)
            {
                iteration++;

                CollisionInfo collisionInfo = characterCollisions.CastBody(
                    position,
                    displacement,
                    useFullBody ? 0f : StepOffset,
                    in filter
                );


                if (collisionInfo.hitInfo.hit)
                {

                    if (CheckOneWayPlatformLayerMask(collisionInfo))
                    {
                        position += displacement;
                        break;
                    }

                    // Physics interaction ---------------------------------------------------------------------------------------
                    if (canPushDynamicRigidbodies)
                    {

                        if (collisionInfo.hitInfo.IsRigidbody)
                        {
                            if (collisionInfo.hitInfo.IsDynamicRigidbody)
                            {
                                bool belongsToGroundRigidbody = false;

                                if (Is2D)
                                {
                                    if (GroundCollider2D != null)
                                        if (GroundCollider2D.attachedRigidbody != null)
                                            if (GroundCollider2D.attachedRigidbody != collisionInfo.hitInfo.rigidbody2D)
                                                belongsToGroundRigidbody = true;
                                }
                                else
                                {
                                    if (GroundCollider3D != null)
                                        if (GroundCollider3D.attachedRigidbody != null)
                                            if (GroundCollider3D.attachedRigidbody == collisionInfo.hitInfo.rigidbody3D)
                                                belongsToGroundRigidbody = true;
                                }


                                if (!belongsToGroundRigidbody)
                                {


                                    bool canPushThisObject = CustomUtilities.BelongsToLayerMask(collisionInfo.hitInfo.layer, pushableRigidbodyLayerMask);
                                    if (canPushThisObject)
                                    {
                                        // Use the entire displacement and stop the collide and slide.
                                        position += displacement;
                                        break;
                                    }
                                }
                            }

                        }
                    }

                    //-----------------------------------------------------------------------------------------------------------


                    if (slideOnWalls && !Is2D)
                    {
                        position += collisionInfo.displacement;
                        displacement -= collisionInfo.displacement;

                        bool blocked = UpdateCollideAndSlideData(
                            collisionInfo,
                            ref slidingPlaneNormal,
                            ref groundPlaneNormal,
                            ref displacement
                        );
                    }
                    else
                    {
                        if (!WallCollision)
                            position += collisionInfo.displacement;

                        break;
                    }



                }
                else
                {
                    position += displacement;
                    break;
                }

            }

        }


        protected void PostSimulationCollideAndSlide(ref Vector3 position, ref Quaternion rotation, Vector3 displacement, bool useFullBody)
        {

            Vector3 groundPlaneNormal = GroundStableNormal;
            Vector3 slidingPlaneNormal = Vector3.zero;

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesLayerMask,
                false,
                true,
                oneWayPlatformsLayerMask
            );

            int iteration = 0;


            while (iteration < CharacterConstants.MaxPostSimulationSlideIterations)
            {
                iteration++;

                CollisionInfo collisionInfo = characterCollisions.CastBody(
                    position,
                    displacement,
                    useFullBody ? 0f : StepOffset,
                    in filter
                );


                if (collisionInfo.hitInfo.hit)
                {
                    // If it hits something then reset the rotation.
                    rotation = Rotation;

                    if (CheckOneWayPlatformLayerMask(collisionInfo))
                    {
                        position += displacement;
                        break;
                    }

                    //-----------------------------------------------------------------------------------------------------------

                    if (slideOnWalls && !Is2D)
                    {
                        position += collisionInfo.displacement;
                        displacement -= collisionInfo.displacement;

                        // Get the new slide plane.
                        bool blocked = UpdateCollideAndSlideData(
                            collisionInfo,
                            ref slidingPlaneNormal,
                            ref groundPlaneNormal,
                            ref displacement
                        );
                    }
                    else
                    {
                        if (!WallCollision)
                            position += collisionInfo.displacement;

                        break;
                    }


                }
                else
                {
                    position += displacement;
                    break;
                }

            }

        }

        /// <summary>
        /// Returns true if the current ground layer is considered as a one way platform.
        /// </summary>
        public bool IsGroundAOneWayPlatform => CustomUtilities.BelongsToLayerMask(GroundObject.layer, oneWayPlatformsLayerMask);


        public bool CheckOneWayPlatformLayerMask(CollisionInfo collisionInfo)
        {
            int collisionLayer = collisionInfo.hitInfo.layer;
            return CustomUtilities.BelongsToLayerMask(collisionLayer, oneWayPlatformsLayerMask);
        }

        public bool CheckOneWayPlatformCollision(Vector3 contactPoint, Vector3 characterPosition)
        {
            Vector3 contactPointToBottom = GetBottomCenter(characterPosition) - contactPoint;

            float collisionAngle = Is2D ? Vector2.Angle(Up, contactPointToBottom) : Vector3.Angle(Up, contactPointToBottom);
            return collisionAngle <= 0.5f * oneWayPlatformsValidArc;
        }

        public bool CanEnterGroundedState => !alwaysNotGrounded && forceNotGroundedFrames == 0;

        protected void UnstableCollideAndSlide(ref Vector3 position, Vector3 displacement, float dt)
        {

            HitInfoFilter filter = new HitInfoFilter(
                ObstaclesLayerMask,
                false,
                true,
                oneWayPlatformsLayerMask
            );


            int iteration = 0;

            // Used to determine if the character should collide (or not) with the OWP.
            bool isValidOWP = false;

            bool bottomCollision = false;

            Vector3 slidePlaneANormal = Vector3.zero;
            Vector3 slidePlaneBNormal = Vector3.zero;

            while (iteration < CharacterConstants.MaxSlideIterations || displacement == Vector3.zero)
            {
                iteration++;


                CollisionInfo collisionInfo = characterCollisions.CastBody(
                    position,
                    displacement,
                    0f,
                    in filter
                );

                if (collisionInfo.hitInfo.hit)
                {
                    float slopeAngle = Vector3.Angle(Up, collisionInfo.hitInfo.normal);
                    bool stableHit = slopeAngle <= slopeLimit;
                    bottomCollision = slopeAngle < 90f;

                    if (CheckOneWayPlatformLayerMask(collisionInfo))
                    {
                        // Check if the character hits this platform with the bottom part of the capsule
                        Vector3 nextPosition = position + collisionInfo.displacement;

                        isValidOWP = CheckOneWayPlatformCollision(collisionInfo.hitInfo.point, nextPosition);

                        if (!isValidOWP)
                        {
                            position += displacement;
                            break;
                        }
                    }

                    if (canPushDynamicRigidbodies)
                    {
                        if (collisionInfo.hitInfo.IsRigidbody)
                        {
                            if (collisionInfo.hitInfo.IsDynamicRigidbody)
                            {
                                bool canPushThisObject = CustomUtilities.BelongsToLayerMask(collisionInfo.hitInfo.layer, pushableRigidbodyLayerMask);
                                if (canPushThisObject)
                                {
                                    position += displacement;
                                    break;
                                }
                            }

                        }
                    }

                    // Fall back to this
                    position += collisionInfo.displacement;
                    displacement -= collisionInfo.displacement;

                    // Determine the displacement vector and store the slide plane A
                    if (slidePlaneANormal == Vector3.zero)
                    {

                        if (preventUnstableClimbing && bottomCollision && !stableHit)
                        {
                            bool isUpwardsDisplacement = transform.InverseTransformVectorUnscaled(Vector3.Project(displacement, Up)).y > 0f;

                            if (isUpwardsDisplacement)
                                displacement = Vector3.Project(displacement, Up);
                            else
                                displacement = Vector3.ProjectOnPlane(displacement, collisionInfo.hitInfo.normal);
                        }
                        else
                        {
                            displacement = Vector3.ProjectOnPlane(displacement, collisionInfo.hitInfo.normal);
                        }


                        // store the slide plane A
                        slidePlaneANormal = collisionInfo.hitInfo.normal;


                    }
                    else if (slidePlaneBNormal == Vector3.zero)
                    {

                        slidePlaneBNormal = collisionInfo.hitInfo.normal;
                        Vector3 displacementDirection = Vector3.Cross(slidePlaneANormal, slidePlaneBNormal);
                        displacementDirection.Normalize();

                        displacement = Vector3.Project(displacement, displacementDirection);

                    }

                }
                else
                {
                    position += displacement;
                    break;
                }

            }

            UnstableProbeGround(position, isValidOWP, dt);

        }



        void UnstableProbeGround(Vector3 position, bool isValidOWP, float dt)
        {
            if (!CanEnterGroundedState)
            {
                unstableGroundContactTime = 0f;
                PredictedGround = null;
                PredictedGroundDistance = 0f;

                characterCollisionInfo.ResetGroundInfo();

                return;
            }

            HitInfoFilter groundCheckFilter = new HitInfoFilter(
                isValidOWP ? ObstaclesLayerMask : ObstaclesWithoutOWPLayerMask,
                false,
                true
            );

            CollisionInfo collisionInfo = characterCollisions.CheckForGround(
                position,
                StepOffset,
                CharacterConstants.GroundPredictionDistance,
                in groundCheckFilter
            );

            if (collisionInfo.hitInfo.hit)
            {
                PredictedGround = collisionInfo.hitInfo.transform.gameObject;
                PredictedGroundDistance = collisionInfo.displacement.magnitude;
                lastPredictedGroundPosition = PredictedGround.transform.position;
                lastPredictedGroundRotation = PredictedGround.transform.rotation;

                bool isPredictedGroundOneWayPlatform = CheckOneWayPlatformLayerMask(collisionInfo);

                if (isPredictedGroundOneWayPlatform)
                    PhysicsComponent.IgnoreCollision(collisionInfo.hitInfo, true);

                bool validForGroundCheck = PredictedGroundDistance <= CharacterConstants.GroundCheckDistance;

                if (validForGroundCheck)
                {
                    unstableGroundContactTime += dt;

                    bool processGround = false;
                    if (detectGroundWhileAscending)
                    {
                        processGround = true;
                    }
                    else
                    {
                        processGround =
                            IsFalling ||
                            unstableGroundContactTime >= CharacterConstants.MaxUnstableGroundContactTime ||
                            collisionInfo.hitInfo.IsRigidbody;
                    }

                    if (processGround)
                    {
                        ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);
                        characterCollisionInfo.SetGroundInfo(collisionInfo, this);
                    }


                }
                else
                {
                    unstableGroundContactTime = 0f;
                    characterCollisionInfo.ResetGroundInfo();
                }

            }
            else
            {
                unstableGroundContactTime = 0f;
                PredictedGround = null;
                PredictedGroundDistance = 0f;

                characterCollisionInfo.ResetGroundInfo();

            }
        }
        /// <summary>
        /// Gets the object below the character (only valid if the character is falling). The maximum prediction distance is defined by the constant "GroundPredictionDistance".
        /// </summary>
        public GameObject PredictedGround { get; private set; }

        /// <summary>
        /// Gets the distance to the "PredictedGround".
        /// </summary>
        public float PredictedGroundDistance { get; private set; }


        Vector3 lastPredictedGroundPosition;
        Quaternion lastPredictedGroundRotation;

        void ProcessNewGround(Transform newGroundTransform, CollisionInfo collisionInfo)
        {
            bool isThisANewGround = collisionInfo.hitInfo.transform != GroundTransform;
            if (isThisANewGround)
            {
#if UNITY_TERRAIN_MODULE
                CurrentTerrain = terrains.GetOrRegisterValue<Transform, Terrain>(newGroundTransform);
#endif
                groundRigidbodyComponent = groundRigidbodyComponents.GetOrRegisterValue<Transform, RigidbodyComponent>(newGroundTransform);

                if (OnNewGroundEnter != null)
                    OnNewGroundEnter();

            }
        }

        bool UpdateCollideAndSlideData(CollisionInfo collisionInfo, ref Vector3 slidingPlaneNormal, ref Vector3 groundPlaneNormal, ref Vector3 displacement)
        {

            Vector3 normal = collisionInfo.hitInfo.normal;

            if (collisionInfo.contactSlopeAngle > slopeLimit || !IsStableLayer(collisionInfo.hitInfo.layer))
            {

                if (slidingPlaneNormal != Vector3.zero)
                {
                    bool acuteAngleBetweenWalls = Vector3.Dot(normal, slidingPlaneNormal) > 0f;

                    if (acuteAngleBetweenWalls)
                        displacement = CustomUtilities.DeflectVector(displacement, groundPlaneNormal, normal);
                    else
                        displacement = Vector3.zero;

                }
                else
                {
                    displacement = CustomUtilities.DeflectVector(displacement, groundPlaneNormal, normal);
                }

                slidingPlaneNormal = normal;
            }
            else
            {
                displacement = CustomUtilities.ProjectOnTangent(
                    displacement,
                    normal,
                    Up
                );

                groundPlaneNormal = normal;
                slidingPlaneNormal = Vector3.zero;

            }

            return displacement == Vector3.zero;
        }


        void OnDrawGizmos()
        {
            if (CharacterBody == null)
                CharacterBody = GetComponent<CharacterBody>();

            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);

            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 origin = CustomUtilities.Multiply(Vector3.up, stepUpDistance);
            Gizmos.DrawWireCube(
                origin,
                new Vector3(1.1f * CharacterBody.BodySize.x, 0.02f, 1.1f * CharacterBody.BodySize.x)
            );

            Gizmos.matrix = Matrix4x4.identity;

        }


    }


}