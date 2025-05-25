using Modules.Planets;
using Zenject;
using UnityEngine;

public class PlanetPresenter : IInitializable
{
    private readonly Planet _planet;
    private readonly IPlanetView _view;
    private readonly PlanetPopupPresenter _popupPresenter;

    public PlanetPresenter(Planet planet, IPlanetView view, PlanetPopupPresenter popupPresenter)
    {
        _planet = planet;
        _view = view;
        _popupPresenter = popupPresenter;
    }

    public void Initialize()
    {
        Debug.Log($"[PlanetPresenter] Initializing {_planet.Name}");

        _view.SetLocked(!_planet.IsUnlocked);
        _view.SetIcon(_planet.GetIcon(_planet.IsUnlocked));
        _view.SetIncomeLabel($"{_planet.MinuteIncome}/min");
        _view.SetPriceLabel(_planet.Price);

        _view.OnClick(OnClick);
        _view.OnLongPress(OnLongPress);

        _planet.OnUnlocked += OnUnlocked;
        _planet.OnIncomeReady += _view.SetIncomeReady;
        _planet.OnIncomeTimeChanged += _ => _view.SetIncomeProgress(_planet.IncomeProgress);
        _planet.OnIncomeChanged += i =>
        {
            Debug.Log($"[PlanetPresenter] {_planet.Name} income changed: {i}/min");
            _view.SetIncomeLabel($"{i}/min");
        };
        _planet.OnUpgraded += level =>
        {
            Debug.Log($"[PlanetPresenter] {_planet.Name} upgraded to level {level}");
            _view.SetPriceLabel(_planet.Price);
        };
        _planet.OnGathered += income =>
        {
            Debug.Log($"[PlanetPresenter] {_planet.Name} gathered income: {income}");
            _view.PlayCoinAnimation();
        };
    }

    private void OnUnlocked()
    {
        Debug.Log($"[PlanetPresenter] {_planet.Name} unlocked");

        _view.SetLocked(false);
        _view.SetIcon(_planet.GetIcon(true));
        _view.SetPriceLabel(_planet.Price);
    }

    private void OnClick()
    {
        Debug.Log($"[PlanetPresenter] Click on {_planet.Name}");

        if(_planet.IsIncomeReady)
        {
            Debug.Log($"[PlanetPresenter] Gathering income on {_planet.Name}");
            _planet.GatherIncome();
        }
        else if(_planet.CanUnlockOrUpgrade)
        {
            Debug.Log($"[PlanetPresenter] Trying to unlock or upgrade {_planet.Name}");
            _planet.UnlockOrUpgrade();
        }
        else
        {
            Debug.Log($"[PlanetPresenter] {_planet.Name} cannot unlock/upgrade or gather income");
        }
    }

    private void OnLongPress()
    {
        Debug.Log($"[PlanetPresenter] Long press on {_planet.Name} — showing popup");
        _popupPresenter.Show(_planet);
    }
}
