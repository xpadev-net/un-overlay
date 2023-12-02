using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleButtonGroup : MonoBehaviour
{
    public GameObject buttonGroup;
    private List<LocaleButton> buttons = new List<LocaleButton>();

    IEnumerator Start()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;
        
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            var button = LocaleButton.CreateInstance(locale);
            button.transform.SetParent(buttonGroup.transform);
            button.onClick = () =>
            {
                Debug.Log("clicked");
                LocalizationSettings.SelectedLocale = locale;
            };
            button.Init();
            buttons.Add(button);
        }
    }
}