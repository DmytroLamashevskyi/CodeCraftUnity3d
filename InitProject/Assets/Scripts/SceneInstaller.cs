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

    public override void InstallBindings()
    {
        Container.Bind<ISnake>().To<Snake>().FromInstance(_snake).AsSingle();
        Container.Bind<GameCycle>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IGameUI>().To<GameUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IWorldBounds>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ICoin>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IScore>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IDifficulty>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameController>().FromNew().AsSingle();
    }

}
