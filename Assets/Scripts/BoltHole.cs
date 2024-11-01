using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private Material m_Material;
    private Color m_DefaultColor;
    private bool m_IsSelected;
    private bool m_IsHovered;

    [Header("UI References")]
    [SerializeField] private GameObject m_HoldSelectorPrefab;
    private RectTransform m_UIParent;
    
    private GameObject m_ActiveHoldSelector;
    private GameObject[] m_ClimbingHoldPrefabs;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Find the main UI canvas by name using the new method
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Canvas targetCanvas = null;
        
        foreach (Canvas canvas in canvases)
        {
            if (canvas.name == "MainUICanvas") // Change this to match your canvas name
            {
                targetCanvas = canvas;
                break;
            }
        }

        if (targetCanvas != null)
        {
            // Find or create the HoldSelectorParent
            Transform parent = targetCanvas.transform.Find("HoldSelectorParent");
            if (parent == null)
            {
                GameObject holderObj = new GameObject("HoldSelectorParent");
                holderObj.transform.SetParent(targetCanvas.transform, false);
                parent = holderObj.transform;
            }
            m_UIParent = parent.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("MainUICanvas not found in scene!");
        }

        InitializeMaterial();
        LoadClimbingHoldPrefabs();
    }

    private void LoadClimbingHoldPrefabs()
    {
        // Load all prefabs from Resources folder
        m_ClimbingHoldPrefabs = Resources.LoadAll<GameObject>("ClimbingHolds");
    }

    private void OnEnable()
    {
        // Ensure material is initialized when object is enabled
        if (m_Material == null)
        {
            InitializeMaterial();
        }
    }

    private void InitializeMaterial()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        if (m_Renderer == null) return;

        // Create material instance
        Material sourceMaterial = m_CustomMaterial != null ? m_CustomMaterial : m_Renderer.sharedMaterial;
        if (sourceMaterial == null) return;

        // Create new material instance and store default color
        m_Material = new Material(sourceMaterial);
        m_DefaultColor = sourceMaterial.color;

        // Assign the material
        m_Renderer.material = m_Material;
        
        // Force initial visual state
        UpdateVisualState();
    }

    private void OnDestroy()
    {
        // Clean up the material instance
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
        // Ensure material updates in editor
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
        {
            ShowHoldSelector();
        }
        else
        {
            HideHoldSelector();
        }
    }
    #endregion

    #region Private Methods
    private void UpdateVisualState()
    {
        if (m_Material == null || m_Renderer == null)
        {
            InitializeMaterial();
            if (m_Material == null) return;
        }

        // Ensure material is still assigned
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
        
        // Force material update
        m_Renderer.material.color = targetColor;
    }

    private void ShowHoldSelector()
    {
        if (m_ActiveHoldSelector != null) return;
        
        // Instantiate the UI
        m_ActiveHoldSelector = Instantiate(m_HoldSelectorPrefab, m_UIParent);
        
        // Get references to UI components
        var content = m_ActiveHoldSelector.GetComponentInChildren<ContentSizeFitter>().transform;
        var itemPrefab = Resources.Load<GameObject>("UI/HoldSelectorItem");
        
        // Populate the scroll view
        foreach (var holdPrefab in m_ClimbingHoldPrefabs)
        {
            Debug.Log($"Hold prefab: {holdPrefab.name}");
            var item = Instantiate(itemPrefab, content);
            var text = item.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = holdPrefab.name;
            }
        }
    }

    private void HideHoldSelector()
    {
        if (m_ActiveHoldSelector != null)
        {
            Destroy(m_ActiveHoldSelector);
            m_ActiveHoldSelector = null;
        }
    }
    #endregion
} 