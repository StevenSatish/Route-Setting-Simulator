using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private Image m_PreviewImage;
    [SerializeField] private Button m_Button;
    
    private HoldManager m_HoldManager;
    private GameObject m_HoldPrefab;
    #endregion

    #region Public Methods
    public void Initialize(GameObject _holdPrefab, HoldManager _holdManager)
    {
        m_HoldPrefab = _holdPrefab;
        m_HoldManager = _holdManager;
        
        // Generate preview image
        StartCoroutine(GeneratePreviewImage());
        
        m_Button.onClick.AddListener(OnButtonClick);
    }
    #endregion

    #region Private Methods
    private IEnumerator GeneratePreviewImage()
    {
        // Create a temporary camera to render the preview
        GameObject previewCamera = new GameObject("Preview Camera");
        Camera cam = previewCamera.AddComponent<Camera>();
        
        // Position camera and hold for preview
        previewCamera.transform.position = new Vector3(0, 0, -2);
        GameObject previewHold = Instantiate(m_HoldPrefab, Vector3.zero, Quaternion.identity);
        
        // Render to texture
        RenderTexture rt = new RenderTexture(256, 256, 16);
        cam.targetTexture = rt;
        yield return new WaitForEndOfFrame();
        
        // Convert to sprite
        Texture2D tex = new Texture2D(256, 256);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        tex.Apply();
        
        m_PreviewImage.sprite = Sprite.Create(tex, new Rect(0, 0, 256, 256), Vector2.one * 0.5f);
        
        // Cleanup
        Destroy(previewCamera);
        Destroy(previewHold);
        RenderTexture.active = null;
        cam.targetTexture = null;
    }

    private void OnButtonClick()
    {
        m_HoldManager.SelectHold(m_HoldPrefab);
    }
    #endregion
} 