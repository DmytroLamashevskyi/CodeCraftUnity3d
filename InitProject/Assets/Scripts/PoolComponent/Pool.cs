using Assets.Scripts.Weapon.Bullets.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PoolComponent
{   
    public class Pool<T>: MonoBehaviour  where T : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;
        [SerializeField]
        private Transform _world;
        [SerializeField]
        private T _prefab;
        [SerializeField]
        private int _count;
        protected readonly HashSet<T> _activeItems = new();
        protected readonly Queue<T> _items = new(); 

        public void Awake()
        {  
            for(var i = 0; i < _count; i++)
            {
                T bullet = Object.Instantiate(_prefab, _container);
                _items.Enqueue(bullet);
            }
        }

        public void Enqueue(T item)
        { 
            _items.Enqueue(item);
        }
        public T Dequeue()
        {
           return _items.Dequeue();
        }

        public T Activate()
        {
            if(!_items.TryDequeue(out var item))
            {
                item = Object.Instantiate(_prefab);
            }

            item.transform.SetParent(_world);
            _activeItems.Add(item);

            return item;
        }
        public void Deactivate(T item)
        {
            if(_activeItems.Remove(item))
            { 
                item.transform.SetParent(_container);
                _items.Enqueue(item);
            }
        }

    }
}
