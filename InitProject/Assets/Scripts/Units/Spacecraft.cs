using Assets.Scripts.Common;
using Assets.Scripts.Level;
using Assets.Scripts.Weapon;
using System;
using UnityEngine;

namespace Assets.Scripts.Units
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Spacecraft : MonoBehaviour
    {
        [SerializeField]
        private PhysicsLayer _physicsLayer;

        public event Action<Spacecraft> OnDeath;

        public int Health
        {
            get
            {
                if(_health <= 0)
                {
                    OnDeath?.Invoke(this);
                }
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        public int Armor { get => _armor; set => _armor = value; }
        public float Speed { get => _speed; set => _speed = value; }

        [SerializeField]
        private Cannon[] _weapons;
        private float _speed = 5.0f;
        private int _health = 100;
        private int _armor = 1;

        private Rigidbody2D _rigidbody;


        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>(); 
            _weapons = GetComponentsInChildren<Cannon>();
            gameObject.layer = (int)_physicsLayer;
        }

        public void Move(Vector2 direction)
        {
            if(direction == Vector2.zero)
            {
                return;
            }

            if(direction.sqrMagnitude > 0.01f)
            {
                Vector2 moveDirection = direction.normalized;
                Vector2 nextPosition = _rigidbody.position + moveDirection * Speed * Time.fixedDeltaTime;
                if(LevelBounds.Instance.InBounds(nextPosition))
                    _rigidbody.MovePosition(nextPosition);
            }
        }

        public void Shoot(Transform target) => Shoot(target.position);
        public void Shoot(Vector2 target)
        {
            foreach(var weapon in _weapons)
            {
                weapon.Shoot(target);
            }
        }

        public void TakeDamage(int damage)
        {
            Health = Mathf.Max(0, Health - (damage - Armor));
        }
    }
}
