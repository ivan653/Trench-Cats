using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class StatsUIController : MonoBehaviour
{
    public static StatsUIController Instance;

    [SerializeField] private PanelRenderer _panelRenderer;

    private VisualElement _root;
    private Label _fundsLabel;
    private Label _commandPointsLabel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (_panelRenderer != null)
            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnEnable()
    {
        UIEvents.OnFundsUpdated += UpdateFundsValue;
        UIEvents.OnCommandPointsUpdated += UpdateCommandPointsValue;
    }

    private void OnDisable()
    {
        UIEvents.OnFundsUpdated -= UpdateFundsValue;
        UIEvents.OnCommandPointsUpdated -= UpdateCommandPointsValue;
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        _root = root;
        // Кешуємо посилання безпосередньо з існуючої структури UXML
        _fundsLabel = _root.Q<VisualElement>("FundsContainer")?.Q<Label>("Value");
        _commandPointsLabel = _root.Q<VisualElement>("CommandPointsContainer")?.Q<Label>("Value");
    }

    private void UpdateFundsValue(float value)
    {
        if (_fundsLabel != null)
            _fundsLabel.text = value.ToString();
    }

    private void UpdateCommandPointsValue(float value)
    {
        if (_commandPointsLabel != null)
            _commandPointsLabel.text = value.ToString();
    }
}