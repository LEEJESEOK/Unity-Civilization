using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform target;

    void OnTriggerEnter(Collider collider)
    {
        collider.transform.position = target.position;

        if (collider.attachedRigidbody != null)
            collider.attachedRigidbody.position = target.position;
    }
}
