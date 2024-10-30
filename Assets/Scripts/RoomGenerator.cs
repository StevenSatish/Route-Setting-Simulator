using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    #region Serialized Fields
    [Header("Room Dimensions")]
    [SerializeField, Range(10f, 50f)] private float m_RoomWidth = 20f;
    [SerializeField, Range(5f, 30f)] private float m_RoomHeight = 15f;
    [SerializeField, Range(10f, 50f)] private float m_RoomDepth = 20f;
    
    [Header("Materials")]
    [SerializeField] private Material m_FloorMaterial;
    [SerializeField] private Material m_WallMaterial;
    [SerializeField] private Material m_CeilingMaterial;
    
    [Header("Lighting")]
    [SerializeField] private Color m_AmbientColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private float m_AmbientIntensity = 1f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        GenerateRoom();
    }
    #endregion

    #region Private Methods
    private void GenerateRoom()
    {
        // Floor
        CreateWall(new Vector3(0, 0, 0), 
                  new Vector3(m_RoomWidth, 0.1f, m_RoomDepth), 
                  "Floor", 
                  m_FloorMaterial);
        
        // Ceiling
        CreateWall(new Vector3(0, m_RoomHeight, 0), 
                  new Vector3(m_RoomWidth, 0.1f, m_RoomDepth), 
                  "Ceiling", 
                  m_CeilingMaterial);
        
        // Back wall (climbing wall)
        CreateWall(new Vector3(0, m_RoomHeight/2, -m_RoomDepth/2), 
                  new Vector3(m_RoomWidth, m_RoomHeight, 0.1f), 
                  "BackWall", 
                  m_WallMaterial);
        
        // Side walls
        CreateWall(new Vector3(-m_RoomWidth/2, m_RoomHeight/2, 0), 
                  new Vector3(0.1f, m_RoomHeight, m_RoomDepth), 
                  "LeftWall", 
                  m_WallMaterial);
        CreateWall(new Vector3(m_RoomWidth/2, m_RoomHeight/2, 0), 
                  new Vector3(0.1f, m_RoomHeight, m_RoomDepth), 
                  "RightWall", 
                  m_WallMaterial);
    }

    private void CreateWall(Vector3 _position, Vector3 _scale, string _name, Material _material)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = _name;
        wall.transform.position = _position;
        wall.transform.localScale = _scale;
        wall.transform.SetParent(transform);
        
        if (_material != null)
        {
            wall.GetComponent<Renderer>().material = _material;
        }
    }
    #endregion
}