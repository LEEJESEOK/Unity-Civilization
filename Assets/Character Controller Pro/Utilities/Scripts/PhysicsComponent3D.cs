using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// An implementation of a PhysicsComponent for 3D physics.
    /// </summary>
    public sealed class PhysicsComponent3D : PhysicsComponent
    {
        /// <summary>
        /// Gets the Rigidbody associated with this object.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; } = null;

        /// <summary>
        /// Gets an array with all the colliders associated with this object.
        /// </summary>
        public Collider[] Colliders { get; private set; } = null;


        RaycastHit[] raycastHits = new RaycastHit[10];
        ContactPoint[] contactsBuffer = new ContactPoint[10];

        Collider[] overlappedColliders = new Collider[10];

        protected override void Awake()
        {
            base.Awake();

            Colliders = GetComponentsInChildren<Collider>();
        }

        protected override void Start()
        {
            base.Start();

            Rigidbody = GetComponent<Rigidbody>();
        }



        void OnTriggerStay(Collider other)
        {
            if (ignoreCollisionMessages)
                return;

            if (!other.isTrigger)
                return;

            bool found = false;

            Trigger trigger = new Trigger();

            for (int i = 0; i < Triggers.Count; i++)
            {
                if (Triggers[i].gameObject != other.gameObject)
                    continue;

                found = true;


                // Ignore old Triggers
                if (!Triggers[i].firstContact)
                    continue;

                // Set the firstContact field to false
                trigger = Triggers[i];
                trigger.firstContact = false;
                Triggers[i] = trigger;


                break;

            }

            // First contact
            if (!found)
            {
                trigger.Set(true, other);
                Triggers.Add(trigger);
            }

        }

        void OnTriggerExit(Collider other)
        {
            if (ignoreCollisionMessages)
                return;

            for (int i = Triggers.Count - 1; i >= 0; i--)
            {

                if (Triggers[i].collider3D == other)
                {
                    Triggers.RemoveAt(i);

                    break;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (ignoreCollisionMessages)
                return;

            int bufferHits = collision.GetContacts(contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint contact = contactsBuffer[i];

                Contact outputContact = new Contact(true, contact, collision);

                Contacts.Add(outputContact);
            }


        }

        void OnCollisionStay(Collision collision)
        {
            if (ignoreCollisionMessages)
                return;

            int bufferHits = collision.GetContacts(contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint contact = contactsBuffer[i];

                Contact outputContact = new Contact(false, contact, collision);

                Contacts.Add(outputContact);
            }
        }

        protected override LayerMask GetCollisionLayerMask()
        {
            int objectLayer = gameObject.layer;
            LayerMask output = 0;

            for (int i = 0; i < 32; i++)
            {
                bool exist = !Physics.GetIgnoreLayerCollision(i, objectLayer);

                if (exist)
                    output.value += 1 << i;
            }

            return output;
        }


        public override void IgnoreCollision(in HitInfo hitInfo, bool ignore)
        {
            if (hitInfo.collider3D == null)
                return;

            for (int i = 0; i < Colliders.Length; i++)
                Physics.IgnoreCollision(Colliders[i], hitInfo.collider3D, ignore);
        }

        public override void IgnoreLayerCollision(int targetLayer, bool ignore)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, targetLayer, ignore);

            if (ignore)
                CollisionLayerMask &= ~(1 << targetLayer);
            else
                CollisionLayerMask |= (1 << targetLayer);
        }

        public override void IgnoreLayerMaskCollision(LayerMask layerMask, bool ignore)
        {
            int layerMaskValue = layerMask.value;
            int currentLayer = 1;

            for (int i = 0; i < 32; i++)
            {
                bool exist = (layerMaskValue & currentLayer) > 0;

                if (exist)
                    IgnoreLayerCollision(i, ignore);

                currentLayer <<= 1;
            }

            if (ignore)
                CollisionLayerMask &= ~(layerMask.value);
            else
                CollisionLayerMask |= (layerMask.value);

        }



        // Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        public override bool SimpleRaycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            hits = Physics.Raycast(
                origin,
                Vector3.Normalize(castDisplacement),
                out raycastHits[0],
                castDisplacement.magnitude,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            ) ? 1 : 0;

            bool hit = hits != 0;

            if (hit)
                GetClosestHit(out hitInfo, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hitInfo.hit;
        }

        public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            hits = Physics.RaycastNonAlloc(
                origin,
                Vector3.Normalize(castDisplacement),
                raycastHits,
                castDisplacement.magnitude,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }


        public override int CapsuleCast(out HitInfo hitInfo, Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            hits = Physics.CapsuleCastNonAlloc(
                bottom,
                top,
                radius,
                Vector3.Normalize(castDisplacement),
                raycastHits,
                castDisplacement.magnitude,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }



        public override int SphereCast(out HitInfo hitInfo, Vector3 center, float radius, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            hits = Physics.SphereCastNonAlloc(
                center,
                radius,
                Vector3.Normalize(castDisplacement),
                raycastHits,
                castDisplacement.magnitude,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );

            if (hits != 0)
                GetClosestHit(out hitInfo, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }


        public override int BoxCast(out HitInfo hitInfo, Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, in HitInfoFilter filter)
        {
            hits = BoxCast(center, size, castDisplacement, orientation, in filter);

            if (hits != 0)
                GetClosestHit(out hitInfo, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        public override int BoxCast(Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, in HitInfoFilter filter)
        {
            hits = Physics.BoxCastNonAlloc(
                center,
                size / 2f,
                Vector3.Normalize(castDisplacement),
                raycastHits,
                orientation,
                castDisplacement.magnitude,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );

            UpdateHitsBuffer(castDisplacement);
            return hits;
        }


        // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public override bool OverlapSphere(Vector3 center, float radius, in HitInfoFilter filter)
        {

            hits = Physics.OverlapSphereNonAlloc(
                center,
                radius,
                overlappedColliders,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );

            IgnoreOverlappedColliders(filter.ignorePhysicsLayerMask);

            return hits != 0;
        }

        public override bool OverlapCapsule(Vector3 bottom, Vector3 top, float radius, in HitInfoFilter filter)
        {

            hits = Physics.OverlapCapsuleNonAlloc(
                bottom,
                top,
                radius,
                overlappedColliders,
                filter.collisionLayerMask,
                filter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );

            IgnoreOverlappedColliders(filter.ignorePhysicsLayerMask);

            return hits != 0;
        }


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected override void IgnoreOverlappedColliders(LayerMask ignoredLayerMask)
        {
            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = overlappedColliders[i];

                for (int j = 0; j < Colliders.Length; j++)
                {
                    Collider thisCollider = Colliders[j];

                    if (hitCollider == thisCollider)
                        continue;

                    if (CustomUtilities.BelongsToLayerMask(hitCollider.gameObject.layer, ignoredLayerMask))
                        Physics.IgnoreCollision(thisCollider, hitCollider, true);
                }

            }
        }

        void UpdateHitsBuffer(Vector3 castDisplacement)
        {
            for (int i = 0; i < hits; i++)
            {
                RaycastHit raycastHit = raycastHits[i];
                float raycastHitDistance = raycastHit.distance;
                Collider raycastHitCollider = raycastHit.collider;
                int raycastHitLayer = raycastHit.transform.gameObject.layer;

                HitsBuffer[i] = new HitInfo(ref raycastHit, Vector3.Normalize(castDisplacement));

            }
        }

        protected override void GetClosestHit(out HitInfo hitInfo, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            RaycastHit closestRaycastHit = new RaycastHit();
            closestRaycastHit.distance = Mathf.Infinity;

            bool hit = false;

            for (int i = 0; i < hits; i++)
            {
                RaycastHit raycastHit = raycastHits[i];
                float raycastHitDistance = raycastHit.distance;
                Collider raycastHitCollider = raycastHit.collider;
                int raycastHitLayer = raycastHit.transform.gameObject.layer;

                HitsBuffer[i] = new HitInfo(ref raycastHit, Vector3.Normalize(castDisplacement));

                if (raycastHitDistance == 0)
                    continue;

                bool continueSelf = false;
                for (int j = 0; j < Colliders.Length; j++)
                {
                    Collider thisCollider = Colliders[j];

                    if (raycastHitCollider == thisCollider)
                        continueSelf = true;

                    if (CustomUtilities.BelongsToLayerMask(raycastHitLayer, filter.ignorePhysicsLayerMask))
                        Physics.IgnoreCollision(thisCollider, raycastHitCollider, true);
                }

                if (continueSelf)
                    continue;

                if (raycastHitDistance < filter.minimumDistance || raycastHitDistance > filter.maximumDistance)
                    continue;

                if (filter.ignoreRigidbodies && raycastHitCollider.attachedRigidbody != null)
                    continue;

                hit = true;

                if (raycastHitDistance < closestRaycastHit.distance)
                    closestRaycastHit = raycastHit;

            }

            if (hit)
                hitInfo = new HitInfo(ref closestRaycastHit, Vector3.Normalize(castDisplacement));
            else
                hitInfo = new HitInfo();

        }




    }

}