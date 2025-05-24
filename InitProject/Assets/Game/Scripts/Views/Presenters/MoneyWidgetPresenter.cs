using Modules.Money;

public class MoneyWidgetPresenter
{
    private readonly IMoneyStorage _moneyStorage;
    private readonly IMoneyWidgetView _moneyView;

    public MoneyWidgetPresenter(IMoneyStorage moneyStorage, IMoneyWidgetView moneyView)
    {
        _moneyStorage = moneyStorage;
        _moneyView = moneyView;

        _moneyView.ChangeValue(moneyStorage.Money);

        _moneyStorage.OnMoneyChanged += OnMoneyChanged;
    }

    private void OnMoneyChanged(int newValue, int oldValue)
    {
        _moneyView.ChangeValue(newValue);
    }
}
