using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }

        //本地映射 随时获取用户信息
        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }
        /// <summary>
        /// 当前地图Data
        /// </summary>
        public MapDefine CurrentMapData { get; set; }
        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }
        /// <summary>
        ///当前游戏对象
        /// </summary>
        public GameObject CurrentCharacterObject { get; set; }

    }
}
