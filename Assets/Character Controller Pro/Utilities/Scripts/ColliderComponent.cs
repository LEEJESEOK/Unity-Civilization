using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// This component is an encapsulation of the Collider and Collider2D components, containing the most commonly used 
    /// properties and methods from these components.
    /// </summary>
    public abstract class ColliderComponent : MonoBehaviour
    {
        public abstract Vector3 Size { get; set; }
        public abstract Vector3 Offset { get; set; }
        public abstract Vector3 BoundsSize { get; }
        public abstract float ContactOffset { get; }

        public Vector3 Center => transform.position + transform.TransformVectorUnscaled(Offset);

        protected virtual void Awake()
        {
            this.hideFlags = HideFlags.None;
        }

        public static ColliderComponent CreateInstance(GameObject gameObject, bool includeChildren = true)
        {
            Collider2D collider2D = includeChildren ? gameObject.GetComponentInChildren<Collider2D>() : gameObject.GetComponent<Collider2D>();
            Collider collider3D = includeChildren ? gameObject.GetComponentInChildren<Collider>() : gameObject.GetComponent<Collider>();

            if (collider2D != null)
            {
                // Box collider ------------------------------------------------------------
                BoxCollider2D boxCollider2D = null;

                try
                {
                    boxCollider2D = (BoxCollider2D)collider2D;
                }
                catch (System.Exception) { }

                if (boxCollider2D != null)
                    return gameObject.AddComponent<BoxColliderComponent2D>();


                // Circle collider ------------------------------------------------------------
                CircleCollider2D circleCollider2D = null;

                try
                {
                    circleCollider2D = (CircleCollider2D)collider2D;
                }
                catch (System.Exception) { }

                if (circleCollider2D != null)
                    return gameObject.AddComponent<SphereColliderComponent2D>();

                // Capsule collider ------------------------------------------------------------
                CapsuleCollider2D capsuleCollider2D = null;

                try
                {
                    capsuleCollider2D = (CapsuleCollider2D)collider2D;
                }
                catch (System.Exception) { }

                if (capsuleCollider2D != null)
                    return gameObject.AddComponent<CapsuleColliderComponent2D>();



            }
            else if (collider3D != null)
            {
                // Box collider ------------------------------------------------------------
                BoxCollider boxCollider3D = null;

                try
                {
                    boxCollider3D = (BoxCollider)collider3D;
                }
                catch (System.Exception) { }

                if (boxCollider3D != null)
                    return gameObject.AddComponent<BoxColliderComponent3D>();


                // Circle collider ------------------------------------------------------------
                SphereCollider sphereCollider3D = null;

                try
                {
                    sphereCollider3D = (SphereCollider)collider3D;
                }
                catch (System.Exception) { }

                if (sphereCollider3D != null)
                    return gameObject.AddComponent<SphereColliderComponent3D>();

                // Capsule collider ------------------------------------------------------------
                CapsuleCollider capsuleCollider3D = null;

                try
                {
                    capsuleCollider3D = (CapsuleCollider)collider3D;
                }
                catch (System.Exception) { }

                if (capsuleCollider3D != null)
                    return gameObject.AddComponent<CapsuleColliderComponent3D>();
            }


            return null;

        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();


    }

}