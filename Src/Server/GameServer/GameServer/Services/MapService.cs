using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Managers;

namespace GameServer.Services
{
    class MapService:Singleton<MapService>
    {
        public MapService()
        {

        }

        public void Init()
        {
            MapManager.Instance.Init();
        }
    }
}
