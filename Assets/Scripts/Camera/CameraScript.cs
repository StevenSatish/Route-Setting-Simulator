using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Private Fields
    [SerializeField, Range(0.1f, 10f)] private float m_MoveSpeed = 5f;
    [SerializeField, Range(1f, 10f)] private float m_LookSensitivity = 2f;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private float m_CollisionRadius = 0.5f; // Radius for collision detection
    
    private float m_RotationX = 0f;
    private float m_RotationY = 0f;
    private Vector3 m_Velocity = Vector3.zero;
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
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    #endregion

    #region Private Methods
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;
        
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

        // Reset velocity when not moving
        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            m_Rigidbody.linearVelocity = Vector3.zero;
        }
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * m_LookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * m_LookSensitivity;

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
