using Assets.Scripts.PoolComponent;
using Assets.Scripts.Units;
using System;
using TMPro;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(Spacecraft))]
    public sealed class EnemyController : SpacecraftController
    {   
        public Transform Target { set; get; } 
        public Transform Destination { set; get; }

        void Awake()
        {
            _spacecraft = GetComponent<Spacecraft>();
        }

        protected override void Move()
        {
            Vector3 direction = Destination.position - transform.position;
            _spacecraft.Move(direction);
        }

        protected override void Shoot()
        {
            if(Target is not null)
            {
                _spacecraft.Shoot(Target);
            }
        }

        private void FixedUpdate()
        {
            if ( Vector2.Distance(transform.position, Destination.position) > 0.25)
            {
                Move();
            }
            else
            {
                Shoot();
            }
        }
    }
}