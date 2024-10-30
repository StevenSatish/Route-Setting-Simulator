using System.Collections.Generic;
using UnityEngine;

public class HoldManager : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private GameObject m_HoldButtonPrefab;
    [SerializeField] private Transform m_HoldButtonContainer;
    [SerializeField] private LayerMask m_WallLayer;
    
    private GameObject m_SelectedHoldPrefab;
    private Dictionary<string, GameObject> m_HoldPrefabs = new Dictionary<string, GameObject>();
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        LoadHoldPrefabs();
        CreateHoldButtons();
    }
    #endregion

    #region Private Methods
    private void LoadHoldPrefabs()
    {
        // Load all prefabs from the ClimbingHolds folder
        GameObject[] holdPrefabs = Resources.LoadAll<GameObject>("ClimbingHolds");
        
        foreach (GameObject prefab in holdPrefabs)
        {
            m_HoldPrefabs.Add(prefab.name, prefab);
        }
    }

    private void CreateHoldButtons()
    {
        foreach (var holdPrefab in m_HoldPrefabs)
        {
            GameObject buttonObj = Instantiate(m_HoldButtonPrefab, m_HoldButtonContainer);
            HoldButton holdButton = buttonObj.GetComponent<HoldButton>();
            holdButton.Initialize(holdPrefab.Value, this);
        }
    }
    #endregion

    #region Public Methods
    public void SelectHold(GameObject _holdPrefab)
    {
        m_SelectedHoldPrefab = _holdPrefab;
    }

    public void PlaceHold(Vector3 _position, Quaternion _rotation)
    {
        if (m_SelectedHoldPrefab != null)
        {
            Instantiate(m_SelectedHoldPrefab, _position, _rotation);
        }
    }
    #endregion
} 