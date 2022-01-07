using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Hideable : MonoBehaviour, IHideable 
{
    Renderer renderer;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }
    public void OnFOVEnterHide() {
        renderer.enabled = false;
    }
    public void OnFOVLeaveShow() {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1);
        renderer.enabled = true;
    }
    public void OnFOVTransparency()
    {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.5f);
    }

}
