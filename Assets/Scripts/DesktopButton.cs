using System;
using UnityEngine;
using uWindowCapture;

public class DesktopButton : MonoBehaviour
{
    public int desktopIndex = -1;
    public GameObject canvasObj;
    private GameObject desktopCapture;
    private UwcWindowTexture windowTexture;
    private bool initialized;

    private void Start()
    {
        initialized = false;
    }

    public static DesktopButton CreateInstance(int desktopIndex,GameObject canvasObj) {
        var prefab = Resources.Load<GameObject>("DesktopButtonPrefab");
        var manager = Instantiate(prefab).GetComponent<DesktopButton>();
        manager.desktopIndex = desktopIndex;
        manager.canvasObj = canvasObj;
        return manager;
    }

    public void Init()
    {
        var capturePrefab = Resources.Load<GameObject>("DesktopButtonCapturePrefab");
        desktopCapture = Instantiate(capturePrefab);

        windowTexture = desktopCapture.GetComponent<UwcWindowTexture>();
        Debug.Log("init desktop capture button with "+desktopIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized) return;
        if (!windowTexture || windowTexture.window == null || desktopIndex < 0) {
            return;
        }
        var windowWidth = windowTexture.window.width;
        var windowHeight = windowTexture.window.height;
        windowTexture.desktopIndex = desktopIndex;
        windowTexture.scaleControlType = windowWidth < windowHeight ? WindowTextureScaleControlType.FixedHeight : WindowTextureScaleControlType.FixedWidth;
        var canvasSizeDelta = canvasObj.GetComponent<RectTransform>().sizeDelta;
        var rectTransform = gameObject.GetComponent<RectTransform>();
        desktopCapture.transform.position = canvasObj.transform.position + new Vector3(-canvasSizeDelta.x/2,canvasSizeDelta.y/2,1) + (Vector3)rectTransform.anchoredPosition;
        initialized = true;
    }

    public void OnClick()
    {
        DesktopOverlayManager.CreateInstance(desktopIndex);
    }

    private void OnDestroy()
    {
        Destroy(desktopCapture);
    }
}
