using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{

    private void Start()
    {
        int localeID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(localeID);
    }

    private bool active = false;
    public void ChangeLocale(int _localeID)
    {
        if (active)
            return;
        StartCoroutine(SetLocale(_localeID));
    }
    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        active = false;
    }
}
