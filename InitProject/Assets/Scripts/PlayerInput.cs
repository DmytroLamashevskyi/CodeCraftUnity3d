using Modules;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode LeftKey;
    public KeyCode RightKey;

    private ISnake _snake;

    [Inject]
    public void SetUp(ISnake snake)
    {
        _snake = snake;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    void PlayerMove()
    {  
        if(Input.GetKeyUp(UpKey))
        {
            _snake.Turn(SnakeDirection.UP);
        }else
        if(Input.GetKeyUp(DownKey))
        {
            _snake.Turn(SnakeDirection.DOWN);
        }
        else
        if(Input.GetKeyUp(LeftKey))
        {
            _snake.Turn(SnakeDirection.LEFT);
        }
        else
        if(Input.GetKeyUp(RightKey))
        {
            _snake.Turn(SnakeDirection.RIGHT);
        }
    }
}
