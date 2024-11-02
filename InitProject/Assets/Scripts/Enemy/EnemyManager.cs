using Assets.Scripts.PoolComponent;
using Assets.Scripts.Units;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootEmUp
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField]
        private int _enemyCount = 7;

        [SerializeField]
        private Transform[] _spawnPositions;

        [SerializeField]
        private Transform[] _attackPositions;
        
        [SerializeField]
        private Spacecraft _player;

        [SerializeField]
        private  EnemysPool _pool;
         

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1, 2));
                if(_pool.GetActiveCount < 5)
                {
                    Spawn();
                }
            }
        }

        private void Spawn()
        {
            var enemy = _pool.Activate();

            Transform spawnPosition = RandomPoint(_spawnPositions);
            enemy.transform.position = spawnPosition.position; 
            enemy.Destination = RandomPoint(_attackPositions); 
            enemy.Target = _player.transform; 
            enemy.OnSpacecraftDeath += (_) => SpacecraftDeath(enemy);
        }

        private void SpacecraftDeath(EnemyController enemy)
        {
            enemy.OnSpacecraftDeath -= (_) => SpacecraftDeath(enemy);
            _pool.Deactivate(enemy);
        } 

        private Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}