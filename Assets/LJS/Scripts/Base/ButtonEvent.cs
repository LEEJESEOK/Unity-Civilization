using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonEvent<T> : MonoBehaviour, IButtonEvent<T>
{
    [SerializeField]
    List<ButtonListener<T>> _listenerList;
    public List<ButtonListener<T>> listenerList { get => _listenerList; set => _listenerList = value; }

    public abstract void ClickEvent(T eventType);
}
