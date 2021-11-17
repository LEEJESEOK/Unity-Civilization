using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonEvent<T>
{
    public ButtonListener<T> listener { get; set; }

    void ClickEvent(T eventType);
}
