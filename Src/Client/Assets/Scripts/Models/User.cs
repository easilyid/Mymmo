using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using Entities;
using SkillBridge.Message;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        NUserInfo userInfo;


        public NUserInfo Info
        {
            get { return userInfo; }
        }

        //本地映射 随时获取用户信息
        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }
        /// <summary>
        /// 当前地图Data
        /// </summary>
        public MapDefine CurrentMapData { get; set; }

        public Character CurrentCharacter { get; set; }

        public NCharacterInfo CurrentCharacterInfo { get; set; }
        /// <summary>
        ///当前游戏对象
        /// </summary>
        public PlayerInputController CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacterInfo.Gold += gold;
        }

        public int CurrentRide = 0;

        public int oldRide;
        internal void Ride(int id)
        {
            if (CurrentRide != id)
            {
                CurrentRide = id;
                oldRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }

        public delegate void CharacterInitHandle();

        public event CharacterInitHandle OnCharacterInit;

        internal void CharacterInited()
        {
            if (OnCharacterInit!=null)
            {
                OnCharacterInit();
            }
        }
    }
}
