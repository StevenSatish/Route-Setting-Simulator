using UnityEngine;

public class SelectableHold : MonoBehaviour, ISelectable
{
    private Material[] m_OriginalMaterials;
    private MeshRenderer[] m_Renderers;
    private Color[] m_OriginalColors;
    private Color m_HighlightColor = new Color(1f, 0.8f, 0f, 1f); // Golden highlight
    private bool m_IsRotating = false;
    private Vector3 m_CurrentRotation;
    private Vector3 m_TargetRotation;
    [SerializeField, Range(0.1f, 20f)] private float m_SmoothSpeed = 10f;
    [SerializeField] private float m_RotationMultiplier = 8f;
    private float m_LastCameraXRotation;
    private Camera m_MainCamera;

    private void Awake()
    {
        // Cache renderers and original materials
        m_Renderers = GetComponentsInChildren<MeshRenderer>();
        m_OriginalMaterials = new Material[m_Renderers.Length];
        m_OriginalColors = new Color[m_Renderers.Length];
        
        for (int i = 0; i < m_Renderers.Length; i++)
        {
            m_OriginalMaterials[i] = m_Renderers[i].material;
            m_OriginalColors[i] = m_OriginalMaterials[i].color;
        }
    }

    public void OnHoverEnter()
    {
        foreach (var renderer in m_Renderers)
        {
            Material material = renderer.material;
            material.color = m_HighlightColor;
        }
    }

    public void OnHoverExit()
    {
        if (m_Renderers == null) return;
        
        for (int i = 0; i < m_Renderers.Length; i++)
        {
            if (m_Renderers[i] != null && m_OriginalMaterials[i] != null)
            {
                m_Renderers[i].material.color = m_OriginalColors[i];
            }
        }
    }

    public void OnSelect()
    {
        // Get the associated bolt hole from the ClimbingHold component
        ClimbingHold climbingHold = GetComponent<ClimbingHold>();
        if (climbingHold?.AssociatedBoltHole != null)
        {
            // Show the gallery UI with the associated bolt hole's name
            HoldGalleryUI.Instance.Show(climbingHold.AssociatedBoltHole.name);
            
            // Delete the current hold
            DeleteHold();
        }
    }

    private void Update()
    {
        if (CameraSelector.Instance == null) return;
        
        // If we're rotating but lost mouse button, force stop
        if (m_IsRotating && !Input.GetMouseButton(0))
        {
            StopRotating();
        }

        bool isBeingLookedAt = CameraSelector.Instance.CurrentHoveredObject is MonoBehaviour hoveredMono 
            && hoveredMono.gameObject == gameObject;

        if (isBeingLookedAt)
        {
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                DeleteHold();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartRotating();
            }
        }

        if (m_IsRotating)
        {
            RotateHold();
        }
    }

    private void OnDisable()
    {
        if (m_IsRotating)
        {
            StopRotating();
        }
    }

    private void StartRotating()
    {
        m_IsRotating = true;
        m_MainCamera = Camera.main;
        m_LastCameraXRotation = m_MainCamera.transform.rotation.eulerAngles.x;
        m_CurrentRotation = transform.localRotation.eulerAngles;
        m_TargetRotation = m_CurrentRotation;
    }

    private void StopRotating()
    {
        m_IsRotating = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RotateHold()
    {
        float currentCameraX = m_MainCamera.transform.rotation.eulerAngles.x;
        float rotationDelta = currentCameraX - m_LastCameraXRotation;
        
        // Handle the 360-degree wrap-around
        if (rotationDelta > 180f) rotationDelta -= 360f;
        if (rotationDelta < -180f) rotationDelta += 360f;
        
        if (Mathf.Abs(rotationDelta) > 0.001f)
        {
            m_TargetRotation += Vector3.forward * (rotationDelta * m_RotationMultiplier);
        }
        
        m_CurrentRotation = Vector3.Lerp(m_CurrentRotation, m_TargetRotation, m_SmoothSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(m_CurrentRotation);
        
        m_LastCameraXRotation = currentCameraX;
    }

    private void DeleteHold()
    {
        // Clear hover state before destroying
        m_Renderers = null;
        m_OriginalMaterials = null;

        // Re-enable the associated bolt hole before destroying
        ClimbingHold climbingHold = GetComponent<ClimbingHold>();
        if (climbingHold?.AssociatedBoltHole != null)
        {
            MeshRenderer boltRenderer = climbingHold.AssociatedBoltHole.GetComponent<MeshRenderer>();
            if (boltRenderer != null)
            {
                boltRenderer.enabled = true;
            }
        }

        Destroy(gameObject);
    }
} 