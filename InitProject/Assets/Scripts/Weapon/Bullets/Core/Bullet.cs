using Assets.Scripts.Common;
using Assets.Scripts.Level;
using Assets.Scripts.Units;
using System;
using UnityEngine;

namespace Assets.Scripts.Weapon.Bullets.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Bullet : MonoBehaviour
    {
        public event Action<Bullet> OnCollisionEntered; 

        private BulletData _bulletData;
        private new Rigidbody2D rigidbody2D {  set; get; }

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public Bullet Preapare(BulletData bulletData, PhysicsLayer targetLayer)
        {
            if(bulletData == null)
                throw new ArgumentNullException(nameof(bulletData));

            _bulletData = bulletData; 
            gameObject.layer = (int)targetLayer;
            return this;
        }

        public void Release(Vector2 velocity)
        {
            _spriteRenderer.color = _bulletData != null ? _bulletData.BulletColor : Color.gray;
            rigidbody2D.velocity = velocity * _bulletData.Speed;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            DealDamage(collision.gameObject);
            OnCollisionEntered?.Invoke(this); 
        }

        private void Update()
        {
            if (!LevelBounds.Instance.InBounds(transform.position))
            {
                OnCollisionEntered?.Invoke(this);
            }
        }

        private void DealDamage(GameObject other)
        { 
            if(other.TryGetComponent(out Spacecraft spacecraft))
            {
                spacecraft.TakeDamage(_bulletData.Damage);
            } 
        }
    }
}