using UnityEngine;

public class UnlockPlaceablePath : MonoBehaviour
{
    [SerializeField]
    private LayerMask _newLayer;
    [SerializeField]
    private LayerMask _unitLayer;
    [SerializeField]
    private Collider _triggerCollider;
    [SerializeField]
    private Renderer _renderer;

    private bool _unlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.gameObject.layer == _unitLayer)
            Unlock();
    }
    public void Unlock()
    {
        if (_unlocked)
            return;

        gameObject.layer = _newLayer;
        _renderer.material.EnableKeyword("_EMISSION");
        _unlocked = true;
        _triggerCollider.enabled = false;

        Debug.Log("Unlocked path");
    }
}
