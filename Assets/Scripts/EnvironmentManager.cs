using UnityEngine;
using UnityEngine.Rendering;

public class EnvironmentManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Skybox")]
    [SerializeField] private Material m_SkyboxMaterial;
    [SerializeField] private float m_SkyboxExposure = 1.3f;
    [SerializeField] private Color m_SkyTint = new Color(0.5f, 0.5f, 0.7f);
    [SerializeField] private Color m_GroundColor = new Color(0.369f, 0.349f, 0.341f);
    
    [Header("Fog Settings")]
    [SerializeField] private bool m_EnableFog = true;
    [SerializeField] private Color m_FogColor = Color.gray;
    [SerializeField] private float m_FogStartDistance = 15f;
    [SerializeField] private float m_FogEndDistance = 50f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetupEnvironment();
    }
    #endregion

    #region Private Methods
    private void SetupEnvironment()
    {
        if (m_SkyboxMaterial != null)
        {
            RenderSettings.skybox = m_SkyboxMaterial;
            UpdateSkyboxSettings();
        }

        if (m_EnableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = m_FogColor;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = m_FogStartDistance;
            RenderSettings.fogEndDistance = m_FogEndDistance;
        }
    }

    private void UpdateSkyboxSettings()
    {
        if (m_SkyboxMaterial != null)
        {
            m_SkyboxMaterial.SetFloat("_Exposure", m_SkyboxExposure);
            m_SkyboxMaterial.SetColor("_SkyTint", m_SkyTint);
            m_SkyboxMaterial.SetColor("_GroundColor", m_GroundColor);
        }
    }
    #endregion
}