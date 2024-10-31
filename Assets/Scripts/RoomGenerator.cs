using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoomGenerator : MonoBehaviour
{
    #region Serialized Fields
    [Header("Room Dimensions")]
    [SerializeField, Range(10f, 50f)] private float m_RoomWidth = 20f;
    [SerializeField, Range(5f, 30f)] private float m_RoomHeight = 15f;
    [SerializeField, Range(10f, 50f)] private float m_RoomDepth = 20f;
    
    [Header("Materials")]
    [SerializeField] private Material m_FloorMaterial;
    [SerializeField] private Material m_CeilingMaterial;
    [SerializeField] private Material m_WallMaterial;
    
    [Header("References")]
    [SerializeField] private GameObject m_Floor;
    [SerializeField] private GameObject m_Ceiling;
    [SerializeField] private GameObject m_LeftWall;
    [SerializeField] private GameObject m_RightWall;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        GenerateRoom();
    }
    #endregion

    #region Public Methods
    public void GenerateRoom()
    {
        CleanupExistingWalls();
        
        // Floor
        m_Floor = CreateWall(new Vector3(0, 0, 0), 
                           new Vector3(m_RoomWidth, 0.1f, m_RoomDepth), 
                           "Floor", 
                           m_FloorMaterial);
        
        // Ceiling
        m_Ceiling = CreateWall(new Vector3(0, m_RoomHeight, 0), 
                             new Vector3(m_RoomWidth, 0.1f, m_RoomDepth), 
                             "Ceiling", 
                             m_CeilingMaterial);
        
        // Side walls
        m_LeftWall = CreateWall(new Vector3(-m_RoomWidth/2, m_RoomHeight/2, 0), 
                              new Vector3(0.1f, m_RoomHeight, m_RoomDepth), 
                              "LeftWall", 
                              m_WallMaterial);
        
        m_RightWall = CreateWall(new Vector3(m_RoomWidth/2, m_RoomHeight/2, 0), 
                               new Vector3(0.1f, m_RoomHeight, m_RoomDepth), 
                               "RightWall", 
                               m_WallMaterial);
    }
    #endregion

    #region Private Methods
    private void CleanupExistingWalls()
    {
        if (m_Floor != null) DestroyImmediate(m_Floor);
        if (m_Ceiling != null) DestroyImmediate(m_Ceiling);
        if (m_LeftWall != null) DestroyImmediate(m_LeftWall);
        if (m_RightWall != null) DestroyImmediate(m_RightWall);
    }

    private GameObject CreateWall(Vector3 _position, Vector3 _scale, string _name, Material _material)
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

        return wall;
    }
    #endregion

    #if UNITY_EDITOR
    [CustomEditor(typeof(RoomGenerator))]
    public class RoomGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RoomGenerator generator = (RoomGenerator)target;

            if (GUILayout.Button("Generate Room"))
            {
                generator.GenerateRoom();
            }
        }
    }
    #endif
}