using System;
using UnityEngine;

namespace Assets.Scripts.Level
{
    public sealed class LevelBounds : MonoBehaviour
    { 
        public static LevelBounds Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        [SerializeField]
        private Transform _leftBorder;

        [SerializeField]
        private Transform _rightBorder;

        [SerializeField]
        private Transform _downBorder;

        [SerializeField]
        private Transform _topBorder;
        private static LevelBounds _instance;

        public bool InBounds(Vector3 position)
        {
            var positionX = position.x;
            var positionY = position.y;
            return positionX > this._leftBorder.position.x
                   && positionX < this._rightBorder.position.x
                   && positionY > this._downBorder.position.y
                   && positionY < this._topBorder.position.y;
        }
    }
}