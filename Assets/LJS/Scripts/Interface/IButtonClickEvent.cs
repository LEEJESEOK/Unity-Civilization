using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonEvent<T>
{
    public List<ButtonListener<T>> listenerList { get; set; }

    void ClickEvent(T eventType);
}
