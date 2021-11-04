using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Models;
using UnityEngine;
using Managers;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnEntitySync);
        }


        public int CurrentMapId { get; set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnEntitySync);
        }

        public void Init()
        {

        }
        void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacter == null || (cha.Type==CharacterType.Player&&User.Instance.CurrentCharacter.Id == cha.Id))
                {
                    //当前角色切换地图成功
                    User.Instance.CurrentCharacter = cha;
                }

                CharacterManager.Instance.AddCharacter(cha);
            }

            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave:CharID:{0}", response.entityId);
            if (response.entityId != User.Instance.CurrentCharacter.EntityId)
            {
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            }
            else
            {
                CharacterManager.Instance.Clear();
            }
        }

        void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity,int param)
        {
            Debug.LogFormat("MapEntityUpdateRequest :ID{0} POS:{1} DIR:{2} SPD:{3}", entity.Id, entity.Position.String(), entity.Direction.String(), entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity,
                Param = param
            };
            NetClient.Instance.SendMessage(message);
        }

        void OnEntitySync(object sender, MapEntitySyncResponse response)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse: Entitys:{0}", response.entitySyncs.Count);
            sb.AppendLine();
            foreach (var entity in response.entitySyncs)
            {
                Managers.EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("   [{0}]evt:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }
        public void SendMapTeleport(int teleporterID)
        {
            Debug.LogFormat("MapTeleportRequest :teleporterID:{0}",teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }

    }


}
