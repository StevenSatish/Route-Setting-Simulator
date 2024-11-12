using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Private Fields
    [SerializeField, Range(0.1f, 10f)] private float m_MoveSpeed = 6.5f;
    [SerializeField, Range(1f, 10f)] private float m_LookSensitivity = 2f;
    [SerializeField, Range(0.1f, 1f)] private float m_WebGLSensitivityMultiplier = 0.25f;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private float m_CollisionRadius = 0.5f; // Radius for collision detection
    
    private float m_RotationX = 0f;
    private float m_RotationY = 0f;
    private bool m_IsPointerLocked = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        LockCursor();
    
        if (!TryGetComponent(out m_Rigidbody))
        {
            m_Rigidbody = gameObject.AddComponent<Rigidbody>();
        }
    
        // Configure Rigidbody for camera movement
        m_Rigidbody.useGravity = false;
        m_Rigidbody.linearDamping = 5f; // Add some drag but not infinite
        m_Rigidbody.angularDamping = 5f;
        m_Rigidbody.freezeRotation = true;
        m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Freeze all rotation

        // Add a collider if it doesn't exist
        if (!TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = m_CollisionRadius;
            sphereCollider.isTrigger = false;
        }
    }

    private void Update()
    {
        // Handle pointer lock state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_IsPointerLocked)
            {
                UnlockCursor();
            }
            
            // Hide control panel if it's visible
            if (ControlPanelUI.Instance != null && ControlPanelUI.Instance.m_IsVisible)
            {
                ControlPanelUI.Instance.Hide();
            }
        }
        else if (Input.GetMouseButtonDown(0) && !m_IsPointerLocked) // Left click
        {
            LockCursor();
        }

        // Toggle control panel with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ControlPanelUI.Instance != null)
            {
                ControlPanelUI.Instance.ToggleVisibility();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (ControlPanelUI.Instance != null)
            {
                ControlPanelUI.Instance.Hide();
            }
        }

        // Only handle rotation when pointer is locked
        if (m_IsPointerLocked && !HoldGalleryUI.IsVisible)
        {
            HandleRotation();
        }
    }

    private void FixedUpdate()
    {
        if (!HoldGalleryUI.IsVisible)
        {
            HandleMovement();
        }
        else
        {
            // Reset velocity when gallery is visible
            if (m_Rigidbody != null)
            {
                m_Rigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
    #endregion

    #region Private Methods
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // Add vertical movement for Space and Ctrl
        float upDownInput = 0f;
        if (Input.GetKey(KeyCode.Space))
            upDownInput += 1f;
        if (Input.GetKey(KeyCode.LeftShift))
            upDownInput -= 1f;

        // Combine all movement directions
        Vector3 movement = transform.right * horizontalInput + 
                          transform.forward * verticalInput + 
                          Vector3.up * upDownInput;
        
        // Normalize movement vector
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Calculate desired position
        Vector3 desiredPosition = m_Rigidbody.position + movement * m_MoveSpeed * Time.fixedDeltaTime;

        // Use a layermask that includes all walls
        int layerMask = LayerMask.GetMask("Default"); // Add any other layers you need
        
        // Check for collisions
        if (!Physics.SphereCast(m_Rigidbody.position, m_CollisionRadius, movement.normalized, out RaycastHit hit, 
            movement.magnitude * m_MoveSpeed * Time.fixedDeltaTime, layerMask))
        {
            // No collision, move normally
            m_Rigidbody.MovePosition(desiredPosition);
        }
        else
        {
            // Collision detected, move up to the collision point
            m_Rigidbody.MovePosition(hit.point + hit.normal * m_CollisionRadius);
        }

        // Update velocity reset check to include upDownInput
        if (Mathf.Approximately(horizontalInput, 0f) && 
            Mathf.Approximately(verticalInput, 0f) && 
            Mathf.Approximately(upDownInput, 0f))
        {
            m_Rigidbody.linearVelocity = Vector3.zero;
        }
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * m_LookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * m_LookSensitivity;

        #if UNITY_WEBGL && !UNITY_EDITOR
            mouseX *= m_WebGLSensitivityMultiplier;
            mouseY *= m_WebGLSensitivityMultiplier;
        #endif

        m_RotationY += mouseX;
        m_RotationX -= mouseY;
        m_RotationX = Mathf.Clamp(m_RotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(m_RotationX, m_RotationY, 0f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_IsPointerLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_IsPointerLocked = false;
    }
    #endregion

    #region Unity Messages
    private void OnDrawGizmosSelected()
    {
        // Visualize the collision radius in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_CollisionRadius);
    }
    #endregion
}
