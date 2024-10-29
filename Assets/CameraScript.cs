using UnityEngine;

public class CameraScript : MonoBehaviour
{
    #region Private Fields
    [SerializeField, Range(0.1f, 10f)] private float m_MoveSpeed = 5f;
    [SerializeField, Range(1f, 10f)] private float m_LookSensitivity = 2f;

    private float m_RotationX = 0f;
    private float m_RotationY = 0f;
    private Rigidbody m_Rigidbody;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InitializeRigidbody();
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
    private void InitializeRigidbody()
    {
        m_Rigidbody = gameObject.AddComponent<Rigidbody>();
        m_Rigidbody.useGravity = false;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.freezeRotation = true;
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add a collider to the camera
        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 2f;
        capsuleCollider.radius = 0.5f;
        capsuleCollider.center = Vector3.up;
    }

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
