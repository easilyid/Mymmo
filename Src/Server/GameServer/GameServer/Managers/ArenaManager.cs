using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ArenaManager:Singleton<ArenaManager>
    {
        public const int ArenaMapId = 5; //竞技场的ID的是什么
        public const int MaxInstance = 100; //最大的实例

        private Queue<int> InstanceIndexes = new Queue<int>(); //副本实例的索引
        private Dictionary<int, Arena> Arenas = new Dictionary<int, Arena>();//方便后期维护

        public void Init()
        {
            for (int i = 0; i < MaxInstance; i++)
            {
                InstanceIndexes.Enqueue(i);
            }
        }

        public Arena NewArena(ArenaInfo info, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            var instance = InstanceIndexes.Dequeue();
            var map = MapManager.Instance.GetInstance(ArenaMapId, instance);
            Arena arena = new Arena(map, info, red, blue);
            this.Arenas[instance] = arena;
            arena.PlayerEnter();
            return arena;
        }
    }

}
