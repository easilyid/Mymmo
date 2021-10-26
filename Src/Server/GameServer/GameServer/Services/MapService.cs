using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class MapService:Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);

        }


        public void Init()
        {
            MapManager.Instance.Init();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat($"OnMapEntitySync: characterID:{character.Id}:{character.Info.Name} Entity.Id:{request.entitySync.Id} Event:{request.entitySync.Entity} Entity:{request.entitySync.Entity.String()}");
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync); 
        }

        internal void SendEntityUpdate(NetConnection<NetSession> connection, NEntitySync entity)
        {
            connection.Session.Response.mapEntitySync = new MapEntitySyncResponse();

            connection.Session.Response.mapEntitySync.entitySyncs.Add(entity);

            connection.SendResponse();
        }

        void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport:characterID:{0}:{1} TeleporterId:{2}",character.Id,character.Data,request.teleporterId);

            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Soure TeleporterID [{0}] not existed",request.teleporterId);
                return;
            }

            TeleporterDefine soure = DataManager.Instance.Teleporters[request.teleporterId];
            if (soure.LinkTo==0||!DataManager.Instance.Teleporters.ContainsKey(soure.LinkTo))
            {
                Log.WarningFormat("Soure TeleporterID [{0}] LinkTo ID[{1}] not existed",request.teleporterId,soure.LinkTo);
            }

            TeleporterDefine target = DataManager.Instance.Teleporters[soure.LinkTo];

            MapManager.Instance[soure.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender,character);
        }

    }
}
