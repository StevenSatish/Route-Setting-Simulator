using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClimbingWall : MonoBehaviour
{
    #region Serialized Fields
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int m_GridSize = new Vector2Int(20, 30);
    [SerializeField] private float m_Padding = 1f;
    [SerializeField] private float m_BoltHoleDepth = 0.13f;
    
    [Header("References")]
    [SerializeField] private GameObject m_BoltHolePrefab;
    
    private Transform[,] m_BoltHoles;
    private bool m_IsGenerating = false;
    private bool m_HasInitialized = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (!m_HasInitialized)
        {
            GenerateBoltHoles();
            m_HasInitialized = true;
        }
    }
    #endregion

    #region Private Methods
    public void GenerateBoltHoles()
    {
        if (m_IsGenerating) return;
        m_IsGenerating = true;

        ClearBoltHoles();
        
        m_BoltHoles = new Transform[m_GridSize.x, m_GridSize.y];
        
        float wallWidth = transform.localScale.x;
        float wallHeight = transform.localScale.y;
        float wallThickness = transform.localScale.z;
        
        float availableWidth = wallWidth - (2f * m_Padding);
        float availableHeight = wallHeight - (2f * m_Padding);
        
        float spacingX = availableWidth / (m_GridSize.x - 1);
        float spacingY = availableHeight / (m_GridSize.y - 1);
        
        float startX = (-wallWidth / 2f) + m_Padding;
        float startY = (-wallHeight / 2f) + m_Padding;
        
        for (int x = 0; x < m_GridSize.x; x++)
        {
            for (int y = 0; y < m_GridSize.y; y++)
            {
                Vector3 position = transform.position + new Vector3(
                    startX + (x * spacingX),
                    startY + (y * spacingY),
                    (-wallThickness / 2f) + m_BoltHoleDepth
                );
                
                CreateBoltHole(position, x, y);
            }
        }

        m_IsGenerating = false;
        m_HasInitialized = true;
    }

    private void CreateBoltHole(Vector3 _position, int _x, int _y)
    {
        GameObject boltHole = Instantiate(m_BoltHolePrefab, _position, m_BoltHolePrefab.transform.rotation, transform);
        boltHole.name = $"BoltHole_{_x}_{_y}";
        
        Vector3 prefabScale = m_BoltHolePrefab.transform.localScale;
        boltHole.transform.localScale = new Vector3(
            0.01f,  // Divide by parent's scale
            prefabScale.y / transform.localScale.y,
            0.01f
        );
        
        m_BoltHoles[_x, _y] = boltHole.transform;
    }

    public void ClearBoltHoles()
    {
        // Clear the array first
        if (m_BoltHoles != null)
        {
            for (int x = 0; x < m_BoltHoles.GetLength(0); x++)
            {
                for (int y = 0; y < m_BoltHoles.GetLength(1); y++)
                {
                    if (m_BoltHoles[x, y] != null)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(m_BoltHoles[x, y].gameObject);
                        }
                        else
                        {
                            DestroyImmediate(m_BoltHoles[x, y].gameObject);
                        }
                        m_BoltHoles[x, y] = null;
                    }
                }
            }
            m_BoltHoles = null;
        }
        
        // Clear any remaining bolt holes
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != transform && child.name.StartsWith("BoltHole_"))
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        // Reset initialization flag
        m_HasInitialized = false;
    }
    #endregion

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && !m_IsGenerating)
        {
            EditorApplication.delayCall -= DelayedGeneration;
            EditorApplication.delayCall += DelayedGeneration;
        }
    }

    private void DelayedGeneration()
    {
        if (this == null) return;
        if (!m_HasInitialized)
        {
            GenerateBoltHoles();
        }
    }
    [CustomEditor(typeof(ClimbingWall))]
    public class ClimbingWallEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ClimbingWall wall = (ClimbingWall)target;

            EditorGUILayout.Space(10);  // Add some spacing
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Generate Bolt Holes"))
                {
                    wall.GenerateBoltHoles();
                }
                
                if (GUILayout.Button("Clear Bolt Holes"))
                {
                    wall.ClearBoltHoles();
                }
            }
        }
    }
    #endif
} 