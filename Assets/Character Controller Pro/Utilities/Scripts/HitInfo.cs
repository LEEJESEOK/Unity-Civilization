using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    public readonly struct HitInfo
    {

        public readonly Vector3 normal;

        public readonly Vector3 point;

        public readonly Vector3 direction;

        public readonly float distance;

        public readonly bool hit;

        public readonly Transform transform;

        public readonly Collider2D collider2D;

        public readonly Collider collider3D;

        public readonly Rigidbody2D rigidbody2D;

        public readonly Rigidbody rigidbody3D;

        public readonly int layer;

        public HitInfo(ref RaycastHit raycastHit, Vector3 castDirection) : this()
        {
            if (raycastHit.collider == null)
                return;

            this.hit = true;
            this.point = raycastHit.point;
            this.normal = raycastHit.normal;
            this.distance = raycastHit.distance;
            this.direction = castDirection;

            this.collider3D = raycastHit.collider;

            this.rigidbody3D = this.collider3D.attachedRigidbody;
            this.transform = this.collider3D.transform;
            this.layer = this.transform.gameObject.layer;
        }

        public HitInfo(ref RaycastHit2D raycastHit, Vector3 castDirection) : this()
        {
            if (raycastHit.collider == null)
                return;

            this.hit = true;
            this.point = raycastHit.point;
            this.normal = raycastHit.normal;
            this.distance = raycastHit.distance;
            this.direction = castDirection;

            this.collider2D = raycastHit.collider;

            this.rigidbody2D = this.collider2D.attachedRigidbody;
            this.transform = this.collider2D.transform;
            this.layer = this.transform.gameObject.layer;
        }

        public bool Is2D => collider2D != null;

        public bool IsRigidbody => rigidbody2D != null || rigidbody3D != null;


        public bool IsKinematicRigidbody
        {
            get
            {
                if (rigidbody2D != null)
                    return rigidbody2D.isKinematic;
                else if (rigidbody3D != null)
                    return rigidbody3D.isKinematic;

                return false;
            }
        }

        public bool IsDynamicRigidbody
        {
            get
            {
                if (rigidbody2D != null)
                    return rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
                else if (rigidbody3D != null)
                    return !rigidbody3D.isKinematic;

                return false;
            }
        }

    }

}
