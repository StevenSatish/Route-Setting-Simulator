using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private RectTransform m_CrosshairImage;
    [SerializeField, Range(1f, 100f)] private float m_CrosshairSize = 20f;
    [SerializeField] private Color m_DefaultColor = Color.white;
    [SerializeField] private Color m_HighlightColor = Color.green;
    
    private Image m_Image;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Get the Image component
        if (!TryGetComponent(out m_Image))
        {
            Debug.LogError("CrosshairUI requires an Image component!");
        }

        // Set initial size and color
        if (m_CrosshairImage != null)
        {
            m_CrosshairImage.sizeDelta = new Vector2(m_CrosshairSize, m_CrosshairSize);
        }
        m_Image.color = m_DefaultColor;
    }
    #endregion

    #region Public Methods
    public void SetHighlighted(bool _isHighlighted)
    {
        m_Image.color = _isHighlighted ? m_HighlightColor : m_DefaultColor;
    }
    #endregion
} 