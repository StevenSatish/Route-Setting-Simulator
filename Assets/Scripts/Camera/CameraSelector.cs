using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private float m_MaxSelectionDistance = 100f;
    [SerializeField] private LayerMask m_SelectionMask;
    [SerializeField] private CrosshairUI m_CrosshairUI;
    
    private Camera m_Camera;
    private ISelectable m_CurrentHoveredObject;
    #endregion

    #region Static Fields
    public static CameraSelector Instance { get; private set; }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (!TryGetComponent(out m_Camera))
        {
            Debug.LogError("CameraSelector requires a Camera component!");
        }
    }

    private void Update()
    {
        HandleSelection();
    }
    #endregion

    #region Private Methods
    private void HandleSelection()
    {
        Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, m_MaxSelectionDistance, m_SelectionMask);

        // Check if we're hovering over a selectable object
        if (hitSomething && hitInfo.collider.TryGetComponent(out ISelectable selectable))
        {
            // Update crosshair state
            m_CrosshairUI?.SetHighlighted(true);

            // Handle new hover
            if (m_CurrentHoveredObject != selectable)
            {
                m_CurrentHoveredObject?.OnHoverExit();
                m_CurrentHoveredObject = selectable;
                m_CurrentHoveredObject.OnHoverEnter();
            }

            // Handle 'E' key press instead of mouse click
            if (Input.GetKeyDown(KeyCode.E))
            {
                m_CurrentHoveredObject.OnSelect();
            }
        }
        else
        {
            // Nothing hit, reset states
            m_CrosshairUI?.SetHighlighted(false);
            if (m_CurrentHoveredObject != null)
            {
                m_CurrentHoveredObject.OnHoverExit();
                m_CurrentHoveredObject = null;
            }
        }
    }
    #endregion

    #region Public Properties
    public ISelectable CurrentHoveredObject => m_CurrentHoveredObject;
    #endregion
} 