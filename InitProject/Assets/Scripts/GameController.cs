using Assets.Scripts;
using Modules;
using SnakeGame;
using System;
using UnityEngine;
using Zenject;

public class GameController : ITickable, IDisposable
{
    private readonly ISnake _snake;
    private readonly GameCycle _gameCycle;
    private readonly ICoin _coin;
    private readonly IGameUI _gameUI;
    private readonly IScore _score;
    private readonly IWorldBounds _worldBounds;
    private readonly IDifficulty _difficulty;
    private bool _isGameOver;

    public GameController(
        ISnake snake,
        GameCycle gameCycle,
        ICoin coin,
        IGameUI gameUI,
        IScore score,
        IWorldBounds worldBounds,
        IDifficulty difficulty)
    {
        _snake = snake ?? throw new ArgumentNullException(nameof(snake));
        _gameCycle = gameCycle ?? throw new ArgumentNullException(nameof(gameCycle));
        _coin = coin ?? throw new ArgumentNullException(nameof(coin));
        _gameUI = gameUI ?? throw new ArgumentNullException(nameof(gameUI));
        _score = score ?? throw new ArgumentNullException(nameof(score));
        _worldBounds = worldBounds ?? throw new ArgumentNullException(nameof(worldBounds));
        _difficulty = difficulty ?? throw new ArgumentNullException(nameof(difficulty));

        _snake.OnMoved += HandleSnakeMove;
        _snake.OnSelfCollided += HandleGameOver;
        PlaceCoin();
    }


    public void Dispose()
    {
        _snake.OnMoved -= HandleSnakeMove;
        _snake.OnSelfCollided -= HandleGameOver;
    }

    public void Tick()
    {
        if(!_gameCycle.IsStarted || _isGameOver)
            return;

        SetDifficulty();
    }

    private void HandleSnakeMove(Vector2Int newPosition)
    {
        if(newPosition == _coin.Position)
        {
            CollectCoin();

            if(_score.Current >= 100)
            {
                _isGameOver = true;
                _gameUI.GameOver(true);
            }
        }
    }

    private void CollectCoin()
    {
        _score.Add(_coin.Score);
        _snake.Expand(_coin.Bones);
        PlaceCoin();
    }

    private void HandleGameOver()
    {
        _isGameOver = true;
        _gameUI.GameOver(false);
    }

    private void PlaceCoin()
    {
        _coin.Position = _worldBounds.GetRandomPosition();
        
    }

    private void SetDifficulty()
    {
        if(_difficulty.Current > _score.Current / 10)
        {
            _difficulty.Next(out int diff);
        }
    } 

}
