using UnityEngine;
using Sirenix.OdinInspector;

public class UnlockPlaceablePath : MonoBehaviour
{
    [SerializeField, ValueDropdown("@UnityEditorInternal.InternalEditorUtility.layers", IsUniqueList = true, FlattenTreeView = true, HideChildProperties = true, DropdownHeight = 180)]
    private string _newLayer;
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
        if (FlagsHelper.IsSet(_unitLayer, other.gameObject.layer))
            Unlock();
    }
    public void Unlock()
    {
        if (_unlocked)
            return;

        gameObject.layer = SortingLayer.NameToID(_newLayer);
        _renderer.material.EnableKeyword("_EMISSION");
        _unlocked = true;
        _triggerCollider.enabled = false;

        Debug.Log("Unlocked path");
    }
}
