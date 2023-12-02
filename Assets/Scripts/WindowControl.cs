using System.Collections.Generic;
using UnityEngine;

public class WindowControl : MonoBehaviour {
    public static WindowControl instance;
    
    private List<BaseWindowManager> managers;
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
        managers = new List<BaseWindowManager>();
    }

    public int RegisterWindow(BaseWindowManager self,int width)
    {
        if (self)
        {
            managers.Add(self);
            Debug.Log(self.overlayId);            
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
        foreach (var manager in managers)
        {
            manager.Remove();
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
