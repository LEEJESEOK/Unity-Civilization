using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IButtonListener<T>
{
    T buttonType { get; set; }
    Action<T> onClickCallback { get; set; }

    void SetButtonType(T buttonType);
    void AddClickCallback(Action<T> callback);
    void OnClick();
}
