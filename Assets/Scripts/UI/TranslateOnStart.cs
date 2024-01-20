using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TranslateOnStart : MonoBehaviour
{
    [Serializable]
    private class Translation
    {
        public Language Language;
        public string Text;
    }

    [SerializeField] private Translation[] _translations;

    private readonly Dictionary<Language, string> _translationDict = new();
    private TMP_Text _text;

    private void Awake()
    {
        foreach (var translation in _translations)
            _translationDict.Add(translation.Language, translation.Text);

        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Translate();
    }

    private void OnEnable()
    {
        Translate();
    }

    private void Translate()
    {
        if (GameSettings.Instanse == null)
            return;

        var language = GameSettings.Instanse.Language;
        if (_translationDict.ContainsKey(language))
            _text.text = _translationDict[language];
        else
            Debug.LogError($"There is no translation into the language \"{language}\" for \"{gameObject.name}\"");
    }
}
