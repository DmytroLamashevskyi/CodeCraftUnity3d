using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class LevelBackground : MonoBehaviour
    {
        private float _startPositionY;
        private float _endPositionY;
        private float _movingSpeedY;
        private float _positionX;
        private float _positionZ;

        private Transform _myTransform;

        [SerializeField]
        private Params _params;

        private void Awake()
        {
            this._startPositionY = this._params.m_startPositionY;
            this._endPositionY = this._params.m_endPositionY;
            this._movingSpeedY = this._params.m_movingSpeedY;
            this._myTransform = this.transform;
            var position = this._myTransform.position;
            this._positionX = position.x;
            this._positionZ = position.z;
        }

        private void FixedUpdate()
        {
            if (this._myTransform.position.y <= this._endPositionY)
            {
                this._myTransform.position = new Vector3(
                    this._positionX,
                    this._startPositionY,
                    this._positionZ
                );
            }

            this._myTransform.position -= new Vector3(
                this._positionX,
                this._movingSpeedY * Time.fixedDeltaTime,
                this._positionZ
            );
        }

        [Serializable]
        public sealed class Params
        {
            [SerializeField]
            public float m_startPositionY;

            [SerializeField]
            public float m_endPositionY;

            [SerializeField]
            public float m_movingSpeedY;
        }
    }
}