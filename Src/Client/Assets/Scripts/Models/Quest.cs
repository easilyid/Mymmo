using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Quest
    {
        //本地配置信息
        public QuestDefine Define;
        //网络信息
        public NQuestInfo Info;

        public Quest()
        {

        }
        //构造函数
        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }
        //构造函数
        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }

        public string GetTypeName()
        {
            return EnumUtil.GetEnumDescription(this.Define.Type);
        }

    }
}
