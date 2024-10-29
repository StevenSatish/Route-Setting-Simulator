using UnityEngine;

public class SolidFloor : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private PhysicsMaterial m_FloorMaterial;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeFloor();
    }
    #endregion

    #region Private Methods
    private void InitializeFloor()
    {
        Collider floorCollider = GetComponent<Collider>();
        if (floorCollider != null)
        {
            floorCollider.material = m_FloorMaterial;
        }
        else
        {
            Debug.LogWarning("No Collider found on the floor object. Please add a Collider component.");
        }
    }
    #endregion
}
