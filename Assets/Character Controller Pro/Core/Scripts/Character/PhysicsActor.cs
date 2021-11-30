using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{


    /// <summary>
    /// A physics-based actor that represents a custom 2D/3D interpolated rigidbody.
    /// </summary>
    public abstract class PhysicsActor : MonoBehaviour
    {
        [Header("Rigidbody")]

        [Tooltip("Interpolates the Transform component associated with this actor during Update calls. This is a custom implementation, the actor " +
        "does not use Unity's default interpolation.")]
        public bool interpolateActor = true;

        [Tooltip("Whether or not to use continuous collision detection (rigidbody property). " +
        "This won't affect character vs static obstacles interactions, but it will affect character vs dynamic rigidbodies.")]
        public bool useContinuousCollisionDetection = true;

        [Header("Root motion")]

        [Tooltip("This will activate root motion for the character. With root motion enabled, position and rotation will be handled by the current animation " +
        "clip.")]
        public bool UseRootMotion = false;

        [Space(5f)]


        [Tooltip("Whether or not to transfer position data from the root motion animation to the character.")]
        [Condition("UseRootMotion", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public bool UpdateRootPosition = true;

        [Tooltip("How the root velocity data is going to be applied to the actor.")]
        [Condition(
            new string[] { "UpdateRootPosition", "UseRootMotion" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.ConditionType.IsTrue },
            new float[] { 0f, 0f },
            ConditionAttribute.VisibilityType.NotEditable)]
        public RootMotionVelocityType rootMotionVelocityType = RootMotionVelocityType.SetVelocity;

        [Space(5f)]

        [Tooltip("Whether or not to transfer rotation data from the root motion animation to the character.")]
        [Condition("UseRootMotion", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public bool UpdateRootRotation = true;

        [Tooltip("How the root velocity data is going to be applied to the actor.")]
        // [Condition( "UpdateRootRotation" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
        [Condition(
            new string[] { "UpdateRootRotation", "UseRootMotion" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.ConditionType.IsTrue },
            new float[] { 0f, 0f },
            ConditionAttribute.VisibilityType.NotEditable)]
        public RootMotionRotationType rootMotionRotationType = RootMotionRotationType.AddRotation;



        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Defines how the root velocity data is going to be applied to the actor.
        /// </summary>
        public enum RootMotionVelocityType
        {
            /// <summary>
            /// The root motion velocity will be applied as velocity.
            /// </summary>
            SetVelocity,
            /// <summary>
            /// The root motion velocity will be applied as planar velocity.
            /// </summary>
            SetPlanarVelocity,
            /// <summary>
            /// The root motion velocity will be applied as vertical velocity.
            /// </summary>
            SetVerticalVelocity,
        }


        /// <summary>
        /// Defines how the root rotation data is going to be applied to the actor.
        /// </summary>
        public enum RootMotionRotationType
        {
            /// <summary>
            /// The root motion rotation will override the current rotation.
            /// </summary>
            SetRotation,
            /// <summary>
            /// The root motion rotation will be added to the current rotation.
            /// </summary>
            AddRotation
        }


        /// <summary>
        /// This event is called prior to the physics simulation.
        /// </summary>
        public event System.Action<float> OnPreSimulation;

        /// <summary>
        /// This event is called after the physics simulation.
        /// </summary>
        public event System.Action<float> OnPostSimulation;


        bool interpolationPositionDirtyFlag = false;
        bool interpolationRotationDirtyFlag = false;

        Vector3 startingPosition;
        Vector3 targetPosition;

        Quaternion startingRotation;
        Quaternion targetRotation;


        /// <summary>
        /// Gets the RigidbodyComponent component associated with the character.
        /// </summary>
        public abstract RigidbodyComponent RigidbodyComponent { get; }

        Coroutine postSimulationUpdateCoroutine;

        /// <summary>
        /// Gets the Animator component associated with the state controller.
        /// </summary>
        public Animator Animator { get; private set; }

        AnimatorLink animatorLink = null;


        protected virtual void Awake()
        {
            gameObject.GetOrAddComponent<PhysicsActorSync>();

            Animator animator = this.GetComponentInBranch<CharacterActor, Animator>();
            InitializeAnimation(animator);
        }

        public void SyncBody()
        {
            if (!interpolateActor)
                return;

            startingPosition = targetPosition;
            startingRotation = targetRotation;

            RigidbodyComponent.Position = startingPosition;
            RigidbodyComponent.Rotation = startingRotation;
        }

        /// <summary>
        /// Configures all the animation-related components based on a given Animator component. The Animator provides root motion data along 
        /// </summary>
        public void InitializeAnimation(Animator animator)
        {

            if (animator != null)
            {
                Animator = animator;

                // The update mode must be "animate physics" (physics cycle)
                Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

                animatorLink = Animator.GetComponent<AnimatorLink>();

                if (animatorLink == null)
                    animatorLink = Animator.gameObject.AddComponent<AnimatorLink>();

            }

        }

        public void ResetIKWeights()
        {
            if (animatorLink != null)
                animatorLink.ResetIKWeights();
        }

        protected virtual void OnEnable()
        {
            if (postSimulationUpdateCoroutine == null)
                postSimulationUpdateCoroutine = StartCoroutine(PostSimulationUpdate());


            if (animatorLink != null)
            {
                animatorLink.OnAnimatorMoveEvent += OnAnimatorMoveLinkMethod;
                animatorLink.OnAnimatorIKEvent += OnAnimatorIKLinkMethod;
            }


            ResetInterpolationPosition();
            ResetInterpolationRotation();

        }

        protected virtual void OnDisable()
        {
            if (postSimulationUpdateCoroutine != null)
            {
                StopCoroutine(postSimulationUpdateCoroutine);
                postSimulationUpdateCoroutine = null;
            }

            if (animatorLink != null)
            {
                animatorLink.OnAnimatorMoveEvent -= OnAnimatorMoveLinkMethod;
                animatorLink.OnAnimatorIKEvent -= OnAnimatorIKLinkMethod;
            }
        }

        protected virtual void Start()
        {
            RigidbodyComponent.ContinuousCollisionDetection = useContinuousCollisionDetection;
            RigidbodyComponent.UseInterpolation = false;

            // Interpolation
            targetPosition = startingPosition = transform.position;
            targetRotation = startingRotation = transform.rotation;

        }


        protected virtual void PreSimulationUpdate(float dt) { }
        protected virtual void PostSimulationUpdate(float dt) { }

        public event System.Action<Vector3, Quaternion> OnAnimatorMoveEvent;
        public event System.Action<int> OnAnimatorIKEvent;

        protected virtual void UpdateKinematicRootMotionPosition(Vector3 deltaPosition)
        {
            if (!UpdateRootPosition)
                return;

            RigidbodyComponent.Position += deltaPosition;
        }

        protected virtual void UpdateKinematicRootMotionRotation(Quaternion deltaRotation)
        {
            if (!UpdateRootRotation)
                return;

            if (rootMotionRotationType == RootMotionRotationType.AddRotation)
                RigidbodyComponent.Rotation *= deltaRotation;
            else
                RigidbodyComponent.Rotation = Animator.rootRotation;
        }

        protected virtual void UpdateDynamicRootMotionPosition(Vector3 deltaPosition)
        {
            if (!UpdateRootPosition)
                return;

            RigidbodyComponent.Move(RigidbodyComponent.Position + deltaPosition);
        }

        protected virtual void UpdateDynamicRootMotionRotation(Quaternion deltaRotation)
        {
            if (!UpdateRootRotation)
                return;

            if (rootMotionRotationType == RootMotionRotationType.AddRotation)
                RigidbodyComponent.Rotation *= deltaRotation;
            else
                RigidbodyComponent.Rotation = Animator.rootRotation;
        }

        void OnAnimatorMoveLinkMethod(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            if (!this.enabled)
                return;

            if (!UseRootMotion)
                return;

            if (OnAnimatorMoveEvent != null)
                OnAnimatorMoveEvent(deltaPosition, deltaRotation);


            if (RigidbodyComponent.IsKinematic)
            {
                if (UpdateRootPosition)
                    UpdateKinematicRootMotionPosition(deltaPosition);


                if (UpdateRootRotation)
                    UpdateKinematicRootMotionRotation(deltaRotation);
            }
            else
            {
                if (UpdateRootPosition)
                    UpdateDynamicRootMotionPosition(deltaPosition);

                if (UpdateRootRotation)
                    UpdateDynamicRootMotionRotation(deltaRotation);
            }

            PreSimulationUpdate(Time.deltaTime);

            if (OnPreSimulation != null)
                OnPreSimulation(Time.deltaTime);

            // Manual sync (in case the Transform component is "dirty").
            transform.position = RigidbodyComponent.Position;
            transform.rotation = RigidbodyComponent.Rotation;



        }

        void OnAnimatorIKLinkMethod(int layerIndex)
        {
            if (OnAnimatorIKEvent != null)
                OnAnimatorIKEvent(layerIndex);
        }


        void Update()
        {
            ProcessInterpolation();
        }

        void FixedUpdate()
        {

            // 2D -> Transform and Rigidbody are NOT synced.
            // 3D -> Transform and Rigidbody are synced. 

            if (UseRootMotion)
                return;

            PreSimulationUpdate(Time.deltaTime);

            if (OnPreSimulation != null)
                OnPreSimulation(Time.deltaTime);

            // Manual sync (in case the Transform component is "dirty").
            transform.position = RigidbodyComponent.Position;
            transform.rotation = RigidbodyComponent.Rotation;

        }

        IEnumerator PostSimulationUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;

                float dt = Time.deltaTime;

                if (this.enabled)
                    PostSimulationUpdate(dt);

                UpdateInterpolationData();

                if (OnPostSimulation != null)
                    OnPostSimulation(dt);

            }
        }


        /// <summary>
        /// Returns a value between 0 and 1 that represents the interpolation t factor used by the interpolation algorithm. If the current time value (Time.time)
        /// is close to the most recent fixed time (Time.fixedTime), then the factor will be close to 0. On the other hand, if the current time is close to the next fixed time 
        /// value (Time.fixedTime + Time.fixedDeltaTime) then the factor will be close to 1.
        /// </summary>
        public float InterpolationFactor { get; private set; }

        void ProcessInterpolation()
        {
            if (!interpolateActor)
                return;

            InterpolationFactor = (Time.time - Time.fixedTime) / (Time.fixedDeltaTime);

            transform.position = Vector3.Lerp(startingPosition, targetPosition, InterpolationFactor);
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, InterpolationFactor);
        }

        void UpdateInterpolationData()
        {

            if (!interpolateActor)
                return;


            if (interpolationPositionDirtyFlag)
                interpolationPositionDirtyFlag = false;
            else
                targetPosition = RigidbodyComponent.Position;

            if (interpolationRotationDirtyFlag)
                interpolationRotationDirtyFlag = false;
            else
                targetRotation = RigidbodyComponent.Rotation;
        }


        public void ResetInterpolationPosition()
        {
            interpolationPositionDirtyFlag = true;

            if (RigidbodyComponent != null)
            {
                targetPosition = startingPosition = RigidbodyComponent.Position;
                transform.position = targetPosition;
            }
        }

        public void ResetInterpolationRotation()
        {
            interpolationRotationDirtyFlag = true;

            if (RigidbodyComponent != null)
            {
                targetRotation = startingRotation = RigidbodyComponent.Rotation;
                transform.rotation = targetRotation;
            }
        }



    }

}
