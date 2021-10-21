using GameServer.Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class StatusManager
    {
        public bool HasStatus => this.Status.Count > 0;

        public List<NStatus> Status { get; set; }

        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        public Character Owner { get; set; }

        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            this.Status.Add(new NStatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }

        public void AddGoldChange(int goldDelta)
        {
            if (goldDelta>0)
            {
                AddStatus(StatusType.Money,0,goldDelta,StatusAction.Add);
            }

            if (goldDelta<0)
            {
                AddStatus(StatusType.Money,0,-goldDelta,StatusAction.Delete);
            }
        }

        public void AddItemChange(int id, int count, StatusAction action)
        {
            AddStatus(StatusType.Item, id, count, action);
        }

        public void ApplyResponse(NetMessageResponse message)
        {
            if (message.statusNotify==null)
            {
                message.statusNotify = new StatusNotify();
            }

            foreach (var status in this.Status)
            {
                message.statusNotify.Status.Add(status);
            }
            this.Status.Clear();
            
        }
    }
}
