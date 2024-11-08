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

    public GameObject AssociatedBoltHole { get; set; }

    private void OnDestroy()
    {
        // Re-enable bolt hole renderer when hold is destroyed
        if (AssociatedBoltHole != null)
        {
            MeshRenderer boltRenderer = AssociatedBoltHole.GetComponent<MeshRenderer>();
            if (boltRenderer != null)
            {
                boltRenderer.enabled = true;
            }
        }
    }
} 