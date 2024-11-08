using UnityEngine;

public class ControlPanelUI : MonoBehaviour
{
    #region Private Fields
    public static ControlPanelUI Instance { get; private set; }
    private bool m_IsVisible = true;
    [SerializeField] private GameObject m_ControlPanel; // Reference to the panel containing controls
    [SerializeField] private GameObject m_ReminderText; // Reference to the separate reminder text
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Show(); // Start visible
    }
    #endregion

    #region Public Methods
    public void ToggleVisibility()
    {
        if (m_IsVisible)
            Hide();
        else
            Show();
    }
    #endregion

    #region Private Methods
    private void Show()
    {
        m_ControlPanel.SetActive(true);
        m_IsVisible = true;
        if (m_ReminderText != null)
        {
            m_ReminderText.SetActive(false);
        }
    }

    public void Hide()
    {
        m_ControlPanel.SetActive(false);
        m_IsVisible = false;
        if (m_ReminderText != null)
        {
            m_ReminderText.SetActive(true);
        }
    }
    #endregion
} 