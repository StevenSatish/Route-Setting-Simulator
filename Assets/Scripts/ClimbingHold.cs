using UnityEngine;

public class ClimbingHold : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private Collider m_Collider;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (m_MeshRenderer == null)
            m_MeshRenderer = GetComponent<MeshRenderer>();
        if (m_Collider == null)
            m_Collider = GetComponent<Collider>();
    }
    #endregion

    #region Public Methods
    public void Initialize()
    {
        // Add any initialization logic here
    }
    #endregion
} 