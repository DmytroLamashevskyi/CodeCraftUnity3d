using ShootEmUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.PoolComponent
{
    public class EnemysPool: Pool<EnemyController>
    {
        public int GetActiveCount => _activeItems.Count;
    }
}
