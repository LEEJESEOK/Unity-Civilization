using UnityEngine;

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
        renderer.enabled = true;
    }
}
