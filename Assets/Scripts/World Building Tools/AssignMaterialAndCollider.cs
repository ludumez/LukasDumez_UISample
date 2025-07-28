using UnityEngine;

public class AssignMaterialAndCollider : MonoBehaviour
{
    [SerializeField] private Material _targetMaterial;

    [ContextMenu("Set material and collider")]
    public void Assign()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            if (!renderer.gameObject.TryGetComponent<MeshCollider>(out var collider))
                renderer.gameObject.AddComponent<MeshCollider>();

            renderer.material = _targetMaterial;
        }
    }
}
