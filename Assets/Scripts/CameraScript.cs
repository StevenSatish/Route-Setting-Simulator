using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Private Fields
    [SerializeField, Range(0.1f, 10f)] private float m_MoveSpeed = 5f;
    [SerializeField, Range(1f, 10f)] private float m_LookSensitivity = 2f;
    [SerializeField] private Rigidbody m_Rigidbody;  
    private float m_RotationX = 0f;
    private float m_RotationY = 0f;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    
        // Either get existing Rigidbody or add one if missing
        if (!TryGetComponent(out m_Rigidbody))
        {
        m_Rigidbody = gameObject.AddComponent<Rigidbody>();
    }
    
    // Configure Rigidbody
    m_Rigidbody.useGravity = false;
    m_Rigidbody.isKinematic = false;
    m_Rigidbody.freezeRotation = true;
    m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

    CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
       if (capsuleCollider == null)
       {
           capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
           capsuleCollider.height = 2f;
           capsuleCollider.radius = 0.5f;
           capsuleCollider.center = Vector3.up;
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
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement * m_MoveSpeed * Time.fixedDeltaTime);
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
}
