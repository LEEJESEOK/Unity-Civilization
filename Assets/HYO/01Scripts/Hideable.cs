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

        if (!renderer) return;
        renderer.enabled = false;
    }
    public void OnFOVLeaveShow() {

        if (!renderer) return;
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1);
        renderer.enabled = true;
    }
    public void OnFOVTransparency()
    {
        if (!renderer) return;
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.5f);
    }
    public void OnFOVHideUnits()
    {
        gameObject.GetComponent<Unit>().body.SetActive(false);
    }
    public void OnFOVShowUnits()
    {
        gameObject.GetComponent<Unit>().body.SetActive(true);
    }

}
