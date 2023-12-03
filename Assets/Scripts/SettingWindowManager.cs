using EasyLazyLibrary;
using Unity.VisualScripting;
using UnityEngine;
using uWindowCapture;

public class SettingWindowManager : BaseWindowManager {
    [SerializeField]private GameObject canvas;
    private RectTransform canvasRectTransform;
    private RenderTexture targetTexture;
    [SerializeField]private GameObject overlayCameraObj;
    private Camera overlayCamera;
    [SerializeField]private GameObject overlaySystem;
    private CameraAdjuster cameraAdjuster;
    public delegate void OnDeleteCallback();
    public OnDeleteCallback deleteCallback;

    private int windowWidth;
    private int windowHeight;
    private bool initialized;
    public static SettingWindowManager CreateInstance() {
        var prefab = Resources.Load<GameObject>("SettingOverlayManager");
        var manager = Instantiate(prefab).GetComponent<SettingWindowManager>();
        manager.Init();
        return manager;
    }

    private void Init() {
        Debug.Log("SettingOverlayManager Init Start");
        overlayId = Const.overlayKeyPrefix + "." + Uuid.GetUuid();
        initialized = false;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        overlayCamera = overlayCameraObj.GetComponent<Camera>();
        easyOverlay = overlaySystem.GetComponent<EasyOpenVROverlayForUnity>();
        cameraAdjuster = overlaySystem.GetComponent<CameraAdjuster>();
        easyOverlay.overlayKeyName = overlayId;
        easyOverlay.overlayFriendlyName = overlayId;
        Debug.Log("SettingOverlayManager Init Complete");
    }

    // Update is called once per frame
    private void Update() {
        if (!initialized) {
            initialized = true;
            var sizeDelta = canvasRectTransform.sizeDelta;
            windowWidth = (int)sizeDelta.x;
            windowHeight = (int)sizeDelta.y;
            targetTexture = new RenderTexture(windowWidth, windowHeight, 32);
            overlayCamera.targetTexture = targetTexture;
            easyOverlay.renderTexture = targetTexture;
            canvasRectTransform.sizeDelta = new Vector2(windowWidth, windowHeight);
            var windowItem = new WindowItem(easyOverlay, overlayId, () =>
            {
                Remove();
            });
            transform.position =
                new Vector3(WindowControl.instance.RegisterWindow(windowItem,windowWidth), 0, 0);
            easyOverlay.Init();
            cameraAdjuster.UpdateCameraPov();
        }
    }
    
    public new void Remove()
    {
        Debug.Log("SettingOverlayManager CloseWindow");
        deleteCallback?.Invoke();
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
