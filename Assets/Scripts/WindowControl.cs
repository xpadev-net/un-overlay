using UnityEngine;

public class WindowControl : MonoBehaviour
{
    public static WindowControl instance;

    private bool isHoldingWindow = false;
    private int posLeft = 0;
    private void Awake()
    {
        if( instance is null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        posLeft = 0;
        isHoldingWindow = false;
    }

    public bool TryToGrubWindow()
    {
        if (isHoldingWindow)
            return false;
        isHoldingWindow = true;
        return true;
    }

    public void ReleaseWindow()
    {
        isHoldingWindow = false;
    }

    public int GetWindowLeft(int width)
    {
        var result = posLeft;
        posLeft += width + 50;
        return result + width / 2;
    }
}

