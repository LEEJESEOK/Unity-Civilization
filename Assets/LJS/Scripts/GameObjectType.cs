using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectType : MonoBehaviour
{
    [SerializeField]
    ObjectType _type;
    public ObjectType type { get => _type; set => _type = value; }
}
