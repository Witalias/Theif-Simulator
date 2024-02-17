using System;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<Language, string> _translationDict;
    private TMP_Text _text;

    private void Awake()
    {
        _translationDict = _translations.ToDictionary(translation => translation.Language, translation => translation.Text);
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
        if (GameData.Instanse == null)
            return;

        var language = GameData.Instanse.Language;
        if (_translationDict.ContainsKey(language))
            _text.text = _translationDict[language];
        else
            Debug.LogError($"There is no translation into the language \"{language}\" for \"{gameObject.name}\"");
    }
}
