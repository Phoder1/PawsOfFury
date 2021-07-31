using UnityEngine;
using Sirenix.OdinInspector;

public class UnlockPlaceablePath : MonoBehaviour
{
    [SerializeField, ValueDropdown("@UnityEditorInternal.InternalEditorUtility.layers", IsUniqueList = true, FlattenTreeView = true, HideChildProperties = true, DropdownHeight = 180)]
    private string _newLayer = default;
    [SerializeField]
    private LayerMask _unitLayer = default;
    [SerializeField]
    private Collider _triggerCollider = default;
    [SerializeField]
    private Renderer _renderer = default;

    private bool _unlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (FlagsHelper.IsSet(_unitLayer, other.gameObject.layer.SortingLayerToLayerMask()))
            Unlock();
    }
    public void Unlock()
    {
        if (_unlocked)
            return;

        transform.parent.gameObject.layer = LayerMask.NameToLayer(_newLayer);
        _renderer.material.EnableKeyword("_EMISSION");
        _unlocked = true;
        _triggerCollider.enabled = false;

        Debug.Log("Unlocked path");
    }
}
