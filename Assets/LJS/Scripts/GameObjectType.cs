using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectType : MonoBehaviour
{
    [SerializeField]
    TypeIdBase _type;
    public TypeIdBase type { get => _type; set => _type = value; }
}
