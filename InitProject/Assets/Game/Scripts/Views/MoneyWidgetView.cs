using DG.Tweening;
using TMPro;
using UnityEngine;

public interface IMoneyWidgetView
{
    void ChangeValue(int newValue);
}

public class MoneyWidgetView : MonoBehaviour, IMoneyWidgetView
{
    [SerializeField] private TMP_Text _coins;
    private int _currentValue;

    public void ChangeValue(int newValue)
    {
        DOTween.Kill(this); 
        DOTween.To(() => _currentValue, x =>
        {
            _currentValue = x;
            _coins.text = _currentValue.ToString("N0");
        }, newValue, 0.5f).SetTarget(this);

        _currentValue = newValue;
    }
}
