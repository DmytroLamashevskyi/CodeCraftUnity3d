using Modules.Planets;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Presenters
{
    [CreateAssetMenu(
        fileName = "PresentersInstallers",
        menuName = "Zenject/New PresentersInstallers"
    )]
    public sealed class PresentersInstallers : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMoneyWidgetView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<MoneyWidgetPresenter>().AsSingle().NonLazy();

            Container.Bind<IPlanetPopupView>().FromComponentInHierarchy().AsSingle();
            var popupPresenter = new PlanetPopupPresenter(Container.Resolve<IPlanetPopupView>());
            Container.Bind<PlanetPopupPresenter>().FromInstance(popupPresenter).AsSingle();

            var views = Object.FindObjectsOfType<PlanetView>().OrderBy(v => v.transform.GetSiblingIndex()).ToList();
            var planets = Container.ResolveAll<Planet>().ToList();

            for(int i = 0; i < planets.Count; i++)
            {
                var presenter = new PlanetPresenter(planets[i], views[i], popupPresenter);
                Container.QueueForInject(presenter);
                Container.BindInterfacesTo<PlanetPresenter>().FromInstance(presenter).AsTransient();
            }
        }
    }
}