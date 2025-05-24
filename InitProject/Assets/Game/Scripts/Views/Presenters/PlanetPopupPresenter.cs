using Modules.Planets;

public class PlanetPopupPresenter
{
    private readonly IPlanetPopupView _view;

    private Planet _planet;

    public PlanetPopupPresenter(IPlanetPopupView view)
    {
        _view = view;

        _view.OnClose(() => _view.Hide());
        _view.OnUpgrade(() => TryUpgrade());
    }

    public void Show(Planet planet)
    {
        _planet = planet;

        _view.SetTitle(planet.Name);
        _view.SetIcon(planet.GetIcon(planet.IsUnlocked));
        _view.SetLevel($"Lvl {planet.Level}");
        _view.SetIncome($"{planet.MinuteIncome}/min");
        _view.SetPopulation($"{planet.Population}");
        _view.SetUpgradeButtonVisible(planet.CanUpgrade);
        _view.Show();

        _planet.OnUpgraded += level => _view.SetLevel($"Lvl {level}");
        _planet.OnIncomeChanged += income => _view.SetIncome($"{income}/min");
        _planet.OnPopulationChanged += pop => _view.SetPopulation($"{pop}");
    }

    private void TryUpgrade()
    {
        if(_planet != null && _planet.CanUpgrade)
        {
            _planet.Upgrade();
            _view.SetUpgradeButtonVisible(_planet.CanUpgrade);
        }
    }
}
