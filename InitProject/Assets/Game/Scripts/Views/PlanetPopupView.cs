using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IPlanetPopupView
{
    void SetTitle(string text);
    void SetIcon(Sprite icon);
    void SetPopulation(string value);
    void SetLevel(string value);
    void SetIncome(string value);
    void SetUpgradeButtonVisible(bool isVisible);
    void Show();
    void Hide();

    void OnClose(Action callback);
    void OnUpgrade(Action callback);
}

public class PlanetPopupView : MonoBehaviour, IPlanetPopupView
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _avatar;
    [SerializeField] private TMP_Text _population;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _income;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _upgradeBtn;

    private Action _onClose;
    private Action _onUpgrade;

    public void SetTitle(string text) => _title.text = text;
    public void SetIcon(Sprite icon) => _avatar.sprite = icon;
    public void SetPopulation(string value) => _population.text = value;
    public void SetLevel(string value) => _level.text = value;
    public void SetIncome(string value) => _income.text = value;
    public void SetUpgradeButtonVisible(bool isVisible) => _upgradeBtn.gameObject.SetActive(isVisible);

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void OnClose(Action callback)
    {
        _onClose = callback;
        _closeBtn.onClick.AddListener(() => _onClose?.Invoke());
    }

    public void OnUpgrade(Action callback)
    {
        _onUpgrade = callback;
        _upgradeBtn.onClick.AddListener(() => _onUpgrade?.Invoke());
    }
}
