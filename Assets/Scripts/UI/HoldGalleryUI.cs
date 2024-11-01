using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoldGalleryUI : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private Transform m_ContentParent;
    [SerializeField] private GameObject m_HoldItemPrefab;
    [SerializeField] private float m_ItemSize = 100f;
    [SerializeField] private float m_Spacing = 10f;
    private bool m_IsVisible;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        LoadPreviews();
        Hide(); // Start hidden
    }
    #endregion

    #region Public Methods
    public void Show()
    {
        gameObject.SetActive(true);
        m_IsVisible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        m_IsVisible = false;
    }

    public void Toggle()
    {
        if (m_IsVisible)
            Hide();
        else
            Show();
    }
    #endregion

    #region Private Methods
    private void LoadPreviews()
    {
        // Load all preview sprites
        Sprite[] previews = Resources.LoadAll<Sprite>("HoldPreviews");

        foreach (Sprite preview in previews)
        {
            GameObject item = Instantiate(m_HoldItemPrefab, m_ContentParent);
            
            // Set image
            if (item.TryGetComponent<Image>(out var image))
            {
                image.sprite = preview;
            }

            // Set label
            if (item.GetComponentInChildren<TextMeshProUGUI>() is TextMeshProUGUI label)
            {
                label.text = preview.name.Replace("_preview", "");
            }
        }
    }
    #endregion
} 