using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class LocaleButton : MonoBehaviour
{
    private Locale locale;
    public delegate void OnClickEvent();
    public OnClickEvent onClick;
    private Button button;
    private TMP_Text text;
    public static LocaleButton CreateInstance(Locale locale) {
        var prefab = Resources.Load<GameObject>("LocaleButtonPrefab");
        var manager = Instantiate(prefab).GetComponent<LocaleButton>();
        manager.locale = locale;
        return manager;
    }

    private void Start()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
    
    }

    public void Init()
    {
        if (!button)
        {
            Start();
        }
        text.text = locale.name;
        button.onClick.AddListener(() =>
        {
            Debug.Log("clicked");
            onClick?.Invoke();
        });
    }
}