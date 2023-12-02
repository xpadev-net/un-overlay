using System;
using System.Collections.Generic;
using EasyLazyLibrary;
using UnityEngine;

public struct WindowItem
{
    public EasyOpenVROverlayForUnity easyOverlay;
    
    public delegate void OnCloseCallback();
    public OnCloseCallback onClose;

    public string overlayId;
    public WindowItem(EasyOpenVROverlayForUnity easyOverlay, string overlayId, OnCloseCallback onClose)
    {
        this.easyOverlay = easyOverlay;
        this.overlayId = overlayId;
        this.onClose = onClose;
    }
}

public class WindowControl : MonoBehaviour {
    public static WindowControl instance;
    
    private List<WindowItem> managers;
    private bool isHoldingWindow;
    private int posLeft;
    private void Awake() {
        if (instance is null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        posLeft = 0;
        isHoldingWindow = false;
        managers = new List<WindowItem>();
    }

    public int RegisterWindow(Nullable<WindowItem> item,int width)
    {
        if (item.HasValue)
        {
            managers.Add(item.Value);
            Debug.Log("[WindowControl]added!" +item.Value.overlayId);            
        }
        Debug.Log("added!" + posLeft);
        var result = posLeft;
        posLeft += width + 50;
        return result + width / 2;
    }

    public bool TryToGrubWindow(string selfId,bool isLeft) {
        if (isHoldingWindow) return false;
        if (selfId != GetFrontWindowName(isLeft)) return false;
        isHoldingWindow = true;
        return true;
    }

    public void ReleaseWindow() {
        isHoldingWindow = false;
    }

    public void Reset()
    {
        Debug.Log("[WindowControl]reset!" +managers.Count);
        foreach (var manager in managers)
        {
            manager.onClose();
        }
        managers.Clear();
        posLeft = 0;
        isHoldingWindow = false;
    }
    
    private string GetFrontWindowName(bool isLeft) {
        var shortestDistance = float.MaxValue;
        var managerId = "";
        foreach (var manager in managers)
        {
            var distance = isLeft
                ? manager.easyOverlay.leftHandDistance
                : manager.easyOverlay.rightHandDistance;
            if (distance >= shortestDistance || distance < 0) continue;
            shortestDistance = distance;
            managerId = manager.overlayId;
        }
        return managerId;
    }
}
