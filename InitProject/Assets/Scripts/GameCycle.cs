using UnityEngine;

namespace Assets.Scripts
{
    public class GameCycle : MonoBehaviour
    {
        [SerializeField]
        public bool IsStarted { get; private set; } = true;


        public void StartGame()
        {
            IsStarted = true;
        }

        public void EndGame()
        {
            IsStarted = false;
        }
    }
}
