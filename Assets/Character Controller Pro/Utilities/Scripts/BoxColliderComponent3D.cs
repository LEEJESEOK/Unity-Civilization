using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// This component represents a capsule collider in a 2D world.
    /// </summary>
    public class BoxColliderComponent3D : ColliderComponent3D
    {
        BoxCollider boxCollider = null;


        public override Vector3 Size
        {
            get
            {
                return boxCollider.size;
            }
            set
            {
                boxCollider.size = value;
            }
        }

        public override Vector3 BoundsSize => boxCollider.bounds.size;

        public override Vector3 Offset
        {
            get
            {
                return boxCollider.center;
            }
            set
            {
                boxCollider.center = value;
            }
        }

        protected override void Awake()
        {
            boxCollider = gameObject.GetOrAddComponent<BoxCollider>(true);
            collider = boxCollider;

            base.Awake();

        }


    }



}
