using UnityEngine;

public class Config : MonoBehaviour
{
        
    public static Config i;
    public bool isLeftHand = true;
    private void Awake() {
        if (i is null) {
            i = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        isLeftHand = PlayerPrefs.GetInt("isLeftHand", 1) == 1;
    }
    
    public void SetLeftHand(bool isLeft) {
        isLeftHand = isLeft;
        PlayerPrefs.SetInt("isLeftHand", isLeft ? 1 : 0);
    }
}