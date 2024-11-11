using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Private Fields
    [SerializeField, Range(0.1f, 10f)] private float m_MoveSpeed = 6.5f;
    [SerializeField, Range(5f, 20f)] private float m_LookSensitivity = 8f;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private float m_CollisionRadius = 0.5f; // Radius for collision detection
    
    private float m_RotationX = 0f;
    private float m_RotationY = 0f;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    
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
        if (Input.GetKeyDown(KeyCode.Tab) && ControlPanelUI.Instance != null)
        {
            ControlPanelUI.Instance.ToggleVisibility();
        } else if (Input.GetKeyDown(KeyCode.Escape) && ControlPanelUI.Instance != null)
        {
            ControlPanelUI.Instance.Hide();
        }

        if (!HoldGalleryUI.IsVisible)
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
        // Use delta time to make movement more consistent across platforms
        float mouseX = Input.GetAxisRaw("Mouse X") * m_LookSensitivity * Time.deltaTime * 60f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * m_LookSensitivity * Time.deltaTime * 60f;

        m_RotationY += mouseX;
        m_RotationX -= mouseY;
        m_RotationX = Mathf.Clamp(m_RotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(m_RotationX, m_RotationY, 0f);
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
