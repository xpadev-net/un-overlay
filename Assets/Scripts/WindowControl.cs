using System.Collections.Generic;
using UnityEngine;

public class WindowControl : MonoBehaviour {
    public static WindowControl instance;
    
    private List<OverlayWindowManager> managers;
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
        managers = new List<OverlayWindowManager>();
    }

    public int RegisterWindow(OverlayWindowManager self,int width)
    {
        if (self)
        {
            managers.Add(self);            
        }
        var result = posLeft;
        posLeft += width + 50;
        return result + width / 2;
    }

    public bool TryToGrubWindow(OverlayWindowManager self,bool isLeft) {
        if (isHoldingWindow) return false;
        if (self.id != GetFrontWindowName(isLeft)) return false;
        isHoldingWindow = true;
        return true;
    }

    public void ReleaseWindow() {
        isHoldingWindow = false;
    }
    
    private string GetFrontWindowName(bool isLeft) {
        var shortestDistance = float.MaxValue;
        var managerId = "";
        foreach (var manager in managers)
        {
            var distance = isLeft
                ? manager.easyOpenVROverlay.leftHandDistance
                : manager.easyOpenVROverlay.rightHandDistance;
            if (distance >= shortestDistance || distance < 0) continue;
            shortestDistance = distance;
            managerId = manager.id;
        }
        return managerId;
    }
}
