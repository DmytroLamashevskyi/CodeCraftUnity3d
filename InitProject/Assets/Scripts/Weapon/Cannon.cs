using Assets.Scripts.Common;
using Assets.Scripts.PoolComponent;
using Assets.Scripts.Weapon.Bullets.Core;  
using UnityEngine; 

namespace Assets.Scripts.Weapon
{   
    public class Cannon : MonoBehaviour 
    {
        public BulletData BulletData; 
        public int Ammo { get => _ammo; set => _ammo = value; }  

        [SerializeField]
        private PhysicsLayer _physicsLayer;

        [SerializeField]
        private int _ammo = 10;
        [SerializeField]
        private Bullet _prefab;
        [SerializeField]
        private float _fireRate = 10f;

        private float _cooldownTimer;

        private BulletsPool _pool; 
        
        public void Awake()
        {
            _pool =  FindAnyObjectByType<BulletsPool>();
        }


        private void FixedUpdate()
        {
            if(_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime; 
            }
        }
         
        public void Shoot(Vector2 target)
        {
            if(_cooldownTimer > 0 )
                return; 

            _cooldownTimer = _fireRate;    
            var bullet = _pool.Activate();  
            bullet.OnCollisionEntered += RemoveBullet;
            var vector = target - (Vector2)transform.position;
            bullet.transform.position = transform.position;
            bullet.Preapare(BulletData, _physicsLayer).Release(vector.normalized);
             
        } 

        private void RemoveBullet(Bullet bullet)
        {
            bullet.OnCollisionEntered -= RemoveBullet;
            _pool.Deactivate(bullet);
        }
    }
}
