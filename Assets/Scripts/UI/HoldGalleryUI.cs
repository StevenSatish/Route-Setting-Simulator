using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HoldGalleryUI : MonoBehaviour
{
    #region Private Fields
    public static HoldGalleryUI Instance { get; private set; }
    [SerializeField] private Transform holdGridContainer;// Container with GridLayoutGroup that holds all the holds
    [SerializeField] private GameObject holdItemPrefab;
    [SerializeField] private Color selectedHoldColor = Color.green;
    [SerializeField] private ScrollRect galleryScrollRect;
    [SerializeField] private float scrollSmoothingSpeed = 10f;
    private bool isGalleryVisible;
    private HoldItemUI selectedHoldItem;
    private float targetScrollPosition;
    private List<HoldItemUI> holdItems = new List<HoldItemUI>();
    private string curBoltHoldName = "";
    #endregion

    public class HoldItemUI
    {
        public GameObject gameObject;
        public Image backgroundImage;
        public string previewName;
        public Vector2Int gridPosition;  // For WASD navigation

        public HoldItemUI(GameObject _obj, string _name, Vector2Int _pos)
        {
            gameObject = _obj;
            backgroundImage = _obj.transform.Find("Background")?.GetComponent<Image>();
            previewName = _name;
            gridPosition = _pos;
        }
    }

    #region Unity Lifecycle
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        targetScrollPosition = 1f; // Start at top
        LoadPreviews();
        Hide();
    }

    private void Update()
    {
        if (!isGalleryVisible) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
            return;
        }

        // Add Enter key check
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (selectedHoldItem != null)
            {
                SpawnSelectedHold();
                Hide();
                return;
            }
        }

        Vector2Int moveDirection = Vector2Int.zero;
        
        if (Input.GetKeyDown(KeyCode.W)) moveDirection.y = -1;
        if (Input.GetKeyDown(KeyCode.S)) moveDirection.y = 1;
        if (Input.GetKeyDown(KeyCode.A)) moveDirection.x = -1;
        if (Input.GetKeyDown(KeyCode.D)) moveDirection.x = 1;

        if (moveDirection != Vector2Int.zero && selectedHoldItem != null)
        {
            Vector2Int newPosition = selectedHoldItem.gridPosition + moveDirection;
            HoldItemUI nextItem = holdItems.Find(item => item.gridPosition == newPosition);
            
            if (nextItem != null)
            {
                SelectHoldItem(nextItem);
                Canvas.ForceUpdateCanvases();
                ScrollRectToItem(nextItem.gameObject);
            }
        }

        // Handle smooth scrolling
        if (galleryScrollRect != null)
        {
            float currentScroll = galleryScrollRect.verticalNormalizedPosition;
            float newPosition = Mathf.Lerp(currentScroll, targetScrollPosition, Time.deltaTime * scrollSmoothingSpeed);
            galleryScrollRect.verticalNormalizedPosition = newPosition;
        }
        else
        {
            Debug.LogError("ScrollRect is null!");
        }
    }

    private void ScrollRectToItem(GameObject _item)
    {
        if (galleryScrollRect == null) return;

        RectTransform viewportRect = galleryScrollRect.viewport;
        RectTransform contentRect = holdGridContainer as RectTransform;
        RectTransform itemRect = _item.transform as RectTransform;

        Canvas.ForceUpdateCanvases();
        Vector3[] itemCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];
        itemRect.GetWorldCorners(itemCorners);
        viewportRect.GetWorldCorners(viewportCorners);

        float itemTop = itemCorners[1].y;
        float itemBottom = itemCorners[0].y;
        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        float scrollDelta = 0;
        
        if (itemTop > viewportTop)
        {
            scrollDelta = (itemTop - viewportTop) / (contentRect.rect.height - viewportRect.rect.height);
            targetScrollPosition = Mathf.Clamp01(galleryScrollRect.verticalNormalizedPosition + scrollDelta);
        }
        else if (itemBottom < viewportBottom)
        {
            scrollDelta = (itemBottom - viewportBottom) / (contentRect.rect.height - viewportRect.rect.height);
            targetScrollPosition = Mathf.Clamp01(galleryScrollRect.verticalNormalizedPosition + scrollDelta);
        }
    }

    private void SpawnSelectedHold()
    {
        if (selectedHoldItem == null) return;

        string prefabPath = $"ClimbingHolds/{selectedHoldItem.previewName.Replace("_preview", "")}";
        GameObject holdPrefab = Resources.Load<GameObject>(prefabPath);
        
        if (holdPrefab != null)
        {
            // Spawn with explicit rotation
            Instantiate(holdPrefab, new Vector3(0, 2f, 0), Quaternion.Euler(0, 180, 0));
            Debug.Log($"Spawned hold: {selectedHoldItem.previewName}");
        }
        else
        {
            Debug.LogError($"Could not find hold prefab at path: {prefabPath}");
        }
    }
    #endregion

    #region Public Methods
    public static bool IsVisible { get; private set; }

    public void Show(string _curBoltHoldName)
    {        
        if (holdGridContainer == null)
        {
            Debug.LogError("Hold grid container is null!");
            return;
        }
        
        gameObject.SetActive(true);
        isGalleryVisible = true;
        IsVisible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        curBoltHoldName = _curBoltHoldName;
        
        Debug.Log($"current hold name: {curBoltHoldName} | items count: {holdItems.Count}");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isGalleryVisible = false;
        IsVisible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SelectHoldItem(HoldItemUI _item)
    {
        // Deselect previous
        if (selectedHoldItem != null)
        {
            selectedHoldItem.backgroundImage.color = Color.black;
        }

        // Select new
        selectedHoldItem = _item;
        if (selectedHoldItem != null)
        {
            selectedHoldItem.backgroundImage.color = selectedHoldColor;
        }
    }
    #endregion

    #region Private Methods
    private void LoadPreviews()
    {
        Sprite[] previews = Resources.LoadAll<Sprite>("HoldPreviews");
        Debug.Log($"Found {previews.Length} preview sprites");

        // Calculate grid dimensions based on GridLayoutGroup
        var grid = holdGridContainer.GetComponent<GridLayoutGroup>();
        int columnsCount = grid.constraintCount;

        for (int i = 0; i < previews.Length; i++)
        {
            Sprite preview = previews[i];
            GameObject item = Instantiate(holdItemPrefab, holdGridContainer);

            // Calculate grid position
            Vector2Int gridPos = new Vector2Int(i % columnsCount, i / columnsCount);
            
            // Create HoldItemUI and add to list
            var holdItem = new HoldItemUI(item, preview.name, gridPos);
            holdItems.Add(holdItem);

            // Set up preview image and label as before
            Transform previewImageTransform = item.transform.Find("PreviewImage");
            if (previewImageTransform != null)
            {
                var image = previewImageTransform.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = preview;
                    image.preserveAspect = true;
                    
                    var rectTransform = image.rectTransform;
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.offsetMin = new Vector2(2, 22);
                    rectTransform.offsetMax = new Vector2(-2, -2);
                    
                    image.color = Color.white;
                }
            }

            Transform labelTransform = item.transform.Find("Label");
            if (labelTransform != null)
            {
                var label = labelTransform.GetComponent<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = preview.name.Replace("_preview", "");
                    label.color = Color.white;
                }
            }
        }

        // Select first item by default
        if (holdItems.Count > 0)
        {
            SelectHoldItem(holdItems[0]);
        }
    }

    // Helper method to debug hierarchy
    private string GetHierarchyPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
    #endregion
} 