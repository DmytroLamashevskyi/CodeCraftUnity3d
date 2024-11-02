using ShootEmUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapon.Bullets.Core
{
    [CreateAssetMenu(fileName = "New Bullet Data", menuName = "ScriptableObjects/BulletData", order = 1)]
    public class BulletData : ScriptableObject
    {
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;
        [SerializeField] private Color _bulletColor;

        public float Speed => _speed;
        public int Damage => _damage;
        public Color BulletColor => _bulletColor; 
    }
}
