using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public abstract class SpacecraftController: MonoBehaviour
    {
        public event Action<SpacecraftController> OnSpacecraftDeath;
        protected void OnEnable()
        {
            _spacecraft.OnDeath += (_) => OnDeath();
        }

        [SerializeField]
        protected Spacecraft _spacecraft; 
        protected abstract void Move();
        protected abstract void Shoot();

        private void OnDisable()
        {
            _spacecraft.OnDeath -= (_) => OnDeath();
        }
        protected virtual void OnDeath()
        {
            OnSpacecraftDeath?.Invoke(this);
        }
    }
}
