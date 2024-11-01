using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BoltHole : MonoBehaviour, ISelectable
{
    #region Private Fields
    [Header("Colors")]
    [SerializeField] private Color m_HoverColor = new Color(0.7f, 0.7f, 0.0f);
    [SerializeField] private Color m_SelectedColor = new Color(0.0f, 0.7f, 0.0f);
    
    [Header("References")]
    [SerializeField] private Material m_CustomMaterial;
    
    private MeshRenderer m_Renderer;
    private HoldGalleryUI m_GalleryUI;
    private Material m_Material;
    private Color m_DefaultColor;
    private bool m_IsSelected;
    private bool m_IsHovered;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeMaterial();
    }
    private void Start()
{
        m_GalleryUI = FindFirstObjectByType<HoldGalleryUI>();
    }

    private void OnEnable()
    {
        if (m_Material == null)
        {
            InitializeMaterial();
        }
    }

    private void OnDestroy()
    {
        if (m_Material != null)
        {
            if (Application.isPlaying)
            {
                Destroy(m_Material);
            }
            else
            {
                DestroyImmediate(m_Material);
            }
        }
    }

    private void OnValidate()
    {
        if (m_Material != null)
        {
            UpdateVisualState();
        }
    }
    #endregion

    #region ISelectable Implementation
    public void OnHoverEnter()
    {
        m_IsHovered = true;
        UpdateVisualState();
    }

    public void OnHoverExit()
    {
        m_IsHovered = false;
        UpdateVisualState();
    }

    public void OnSelect()
    {
        m_IsSelected = !m_IsSelected;
        UpdateVisualState();
        if (m_IsSelected)
            m_GalleryUI?.Show();
        else
            m_GalleryUI?.Hide();
    }
    #endregion

    #region Private Methods
    private void InitializeMaterial()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        if (m_Renderer == null) return;

        Material sourceMaterial = m_CustomMaterial != null ? m_CustomMaterial : m_Renderer.sharedMaterial;
        if (sourceMaterial == null) return;

        m_Material = new Material(sourceMaterial);
        m_DefaultColor = sourceMaterial.color;

        m_Renderer.material = m_Material;
        
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (m_Material == null || m_Renderer == null)
        {
            InitializeMaterial();
            if (m_Material == null) return;
        }

        if (m_Renderer.material != m_Material)
        {
            m_Renderer.material = m_Material;
        }

        Color targetColor = m_DefaultColor;
        
        if (m_IsSelected)
        {
            targetColor = m_SelectedColor;
        }
        else if (m_IsHovered)
        {
            targetColor = m_HoverColor;
        }
        
        m_Material.color = targetColor;
        m_Renderer.material.color = targetColor;
    }
    #endregion
} 