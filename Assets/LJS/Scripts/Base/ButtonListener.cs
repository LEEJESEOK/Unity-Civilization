using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ButtonListener<T> : MonoBehaviour, IButtonListener<T>
{
    [SerializeField]
    T _buttonType;
    public T buttonType { get => _buttonType; set => _buttonType = value; }
    public Action<T> onClickCallback { get; set; }


    public void SetButtonType(T buttonType)
    {
        this.buttonType = buttonType;
    }

    public void AddClickCallback(Action<T> callback)
    {
        onClickCallback += callback;
    }

    public void OnClick()
    {
        if (onClickCallback == null)
            return;

        onClickCallback(buttonType);
    }
}
