using Assets.Scripts.Units;
using TMPro;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : SpacecraftController
    {
        [SerializeField] private KeyCode _moveLeftKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode _moveRightKey = KeyCode.RightArrow;
        [SerializeField] private KeyCode _shootKey = KeyCode.Space;

        private void  Awake()
        {
            _spacecraft = GetComponent<Spacecraft>();
            _spacecraft.OnDeath += _ => Time.timeScale = 0;
        }

        void Update()
        {
            Shoot();
            Move();
        } 
        
        protected override void Shoot()
        {
            if(Input.GetKeyDown(_shootKey))
                _spacecraft.Shoot(transform.position + Vector3.up * 3);
        }

        protected override void Move()
        {
            if(Input.GetKey(_moveLeftKey))
                _spacecraft.Move(new Vector2(-1, 0));
            else if(Input.GetKey(_moveRightKey))
                _spacecraft.Move(new Vector2(1, 0));
            else
                _spacecraft.Move(new Vector2(0, 0));
        }

        private void OnDestroy()
        {
            _spacecraft.OnDeath -= _ => Time.timeScale = 0;
        }
    }
}