using EasyLazyLibrary;
using Unity.VisualScripting;
using UnityEngine;
using uWindowCapture;

public class DesktopOverlayManager : MonoBehaviour {
    public int desktopIndex = -1;
    [SerializeField]private GameObject desktopCapture;
    private Transform desktopCaptureTransform;
    [SerializeField]private GameObject canvas;
    private RectTransform canvasRectTransform;
    private UwcWindowTexture windowTexture;
    private RenderTexture targetTexture;
    [SerializeField]private GameObject overlayCameraObj;
    private Camera overlayCamera;
    [SerializeField]private GameObject overlaySystem;
    public EasyOpenVROverlayForUnity easyOverlay;
    public string overlayId;
    private OverlayWindowManager windowManager;

    private int windowWidth;
    private int windowHeight;
    private bool initialized;
    public static DesktopOverlayManager Init(int desktopIndex ) {
        var prefab = Resources.Load<GameObject>("DesktopOverlayManager");
        var manager = Instantiate(prefab).GetComponent<DesktopOverlayManager>();
        manager.desktopIndex = desktopIndex;
        manager.Init();
        return manager;
    }

    private void Init() {
        Debug.Log("Desktop Overlay Manager Init Start");
        overlayId = Const.overlayKeyPrefix + "." + Uuid.GetUuid();
        initialized = false;
        windowTexture = desktopCapture.GetComponent<UwcWindowTexture>();
        desktopCaptureTransform = desktopCapture.GetComponent<Transform>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        overlayCamera = overlayCameraObj.GetComponent<Camera>();
        windowManager = overlaySystem.GetComponent<OverlayWindowManager>();
        easyOverlay = overlaySystem.GetComponent<EasyOpenVROverlayForUnity>();
        easyOverlay.overlayKeyName = overlayId;
        easyOverlay.overlayFriendlyName = overlayId;
        Debug.Log("Desktop Overlay Manager Init Complete");
    }

    // Update is called once per frame
    private void Update() {
        if (!initialized) {
            if (!windowTexture || windowTexture.window == null || desktopIndex < 0) {
                return;
            }
            windowWidth = windowTexture.window.width;
            windowHeight = windowTexture.window.height;
            if (windowHeight + windowWidth < 1) {
                return;
            }
            initialized = true;
            desktopCaptureTransform.localScale = new Vector3(windowWidth, windowHeight);
            targetTexture = new RenderTexture(windowWidth, windowHeight, 32);
            overlayCamera.targetTexture = targetTexture;
            easyOverlay.renderTexture = targetTexture;
            canvasRectTransform.sizeDelta = new Vector2(windowWidth, windowHeight);
            windowTexture.desktopIndex = desktopIndex;
            transform.position =
                new Vector3(WindowControl.instance.RegisterWindow(this,windowWidth), 0, 0);
            windowManager.Init();
            easyOverlay.Init();
        }

        if (windowTexture.desktopIndex != desktopIndex) {
            windowTexture.desktopIndex = desktopIndex;
        }
    }
    
    public void Remove()
    {
        Destroy(gameObject);
    }
}
