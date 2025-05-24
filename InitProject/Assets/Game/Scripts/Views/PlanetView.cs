using Modules.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IPlanetView
{
    void SetIcon(Sprite icon);
    void SetLocked(bool isLocked);
    void SetIncomeProgress(float progress);
    void SetIncomeReady(bool isReady);
    void SetIncomeLabel(string value);
    void SetPriceLabel(string value);
    void PlayCoinAnimation();
    void OnClick(Action callback);
    void OnLongPress(Action onLongPress);
}


public class PlanetView : MonoBehaviour, IPlanetView
{
    [SerializeField] private string _planetName;
    public string PlanetName => _planetName;

    [SerializeField] private Image _icon;
    [SerializeField] private Image _lock;
    [SerializeField] private GameObject _coin;
    [SerializeField] private Image _incomPrgBar;
    [SerializeField] private TMP_Text _incomLabel;
    [SerializeField] private TMP_Text _priceLabel;
    [SerializeField] private SmartButton _smartButton;

    private Action _clickAction;
    private Action _holdAction;

    public void SetIcon(Sprite icon) => _icon.sprite = icon;
    public void SetLocked(bool isLocked) => _lock.enabled = isLocked;
    public void SetIncomeProgress(float progress) => _incomPrgBar.fillAmount = progress;
    public void SetIncomeReady(bool isReady) => _coin.SetActive(isReady);
    public void SetIncomeLabel(string value) => _incomLabel.text = value;
    public void SetPriceLabel(string value) => _priceLabel.text = value;

    public void PlayCoinAnimation()
    {

    }

    public void OnClick(Action callback)
    {
        _clickAction = callback;
        _smartButton.OnClick += _clickAction;
    }

    public void OnLongPress(Action callback)
    {
        _holdAction = callback;
        _smartButton.OnHold += _holdAction;
    }
}
