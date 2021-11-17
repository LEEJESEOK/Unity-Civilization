using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent<T> : MonoBehaviour, IButtonEvent<T>
{
    ButtonListener<T> _listener;
    public ButtonListener<T> listener { get => _listener; set => _listener = value; }

    public void ClickEvent(T eventType)
    {

    }
}
