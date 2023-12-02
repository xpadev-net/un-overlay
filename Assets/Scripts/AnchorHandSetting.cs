using UnityEngine;

public class AnchorHandSetting : MonoBehaviour
{
    public bool isLeft;
    
    public void OnClick()
    {
        Config.i.SetLeftHand(isLeft);
    }
}
