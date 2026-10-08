using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlotUI
{
    private readonly VisualElement _slotRoot;
    private readonly ProgressBar _progressBar;
    private readonly Button _button;
    private readonly IAbility _ability;
    private readonly ArtilleryAbility _artilleryAbility; // За потреби — базовий клас з CooldownEndTime

    public AbilitySlotUI(VisualElement root, IAbility ability, System.Action onClicked)
    {
        _slotRoot = root;
        _ability = ability;
        _artilleryAbility = ability as ArtilleryAbility;

        _progressBar = root.Q<ProgressBar>("ReloadingProgress");
        _button = root.Q<Button>("AbilityButton");

        if (_button != null)
        {
            _button.clicked += onClicked;
        }

        // Встановлюємо діапазон прогрес-бару від 0 до 100% або від 0 до тривалості
        if (_progressBar != null)
        {
            _progressBar.lowValue = 0f;
            _progressBar.highValue = 1f;
        }
    }

    public void UpdateCooldown(double currentServerTime)
    {
        if (_artilleryAbility == null || _progressBar == null) return;

        double endTime = _artilleryAbility.CooldownEndTime.Value;
        double remainingTime = endTime - currentServerTime;

        if (remainingTime <= 0)
        {
            _progressBar.value = 1f; // Перезаряджено
            _progressBar.title = "Ready";
            _button.SetEnabled(true);
        }
        else
        {
            float total = _artilleryAbility.ReloadDuration;
            float progress = 1f - (float)(remainingTime / total);
            
            _progressBar.value = Mathf.Clamp01(progress);
            _progressBar.title = $"{remainingTime:F1}s";
            _button.SetEnabled(false);
        }
    }
}