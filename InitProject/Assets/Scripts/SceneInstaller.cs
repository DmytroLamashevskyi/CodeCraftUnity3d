using Assets.Scripts;
using Modules;
using SnakeGame;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField]
    private Snake _snake;
    [SerializeField]
    private PlayerInput _player;
    [SerializeField]
    private GameCycle  _gameCycle;


    [SerializeField]
    private Coin _coinPrefub;

    public override void InstallBindings()
    {
        Container.Bind<ISnake>().To<Snake>().FromInstance(_snake).AsSingle();
        Container.Bind<GameCycle>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IGameUI>().To<GameUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IWorldBounds>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ICoin>().To<Coin>().FromComponentsInNewPrefab(_coinPrefub).AsCached()
                                    .OnInstantiated<Coin>((context, coin) => coin.Generate());
        Container.Bind<IScore>().To<Score>().FromNew().AsSingle();
        Container.Bind<IDifficulty>().To<Difficulty>().FromNew().AsSingle().WithArguments(1);
        Container.Bind<GameController>().FromNew().AsSingle().NonLazy();
    }

}
