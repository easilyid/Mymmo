using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Battle;
using SkillBridge.Message;

namespace GameServer.AI
{
    //将AI层和逻辑层做隔离
    public class AIAgent
    {
        private Monster monster;
        public AIBase ai;

        public AIAgent(Monster monster)
        {
            this.monster = monster;

            string ainame = monster.Define.AI;
            if (string.IsNullOrEmpty(ainame)) ainame = AIMonsterPassive.ID;

            switch (ainame)
            {
                case AIMonsterPassive.ID:
                    this.ai = new AIMonsterPassive(monster);
                    break;
                case AIBoss.ID:
                    this.ai = new AIBoss(monster);

                    break;
                    
            }
        }

        internal void Update()
        {
            if (ai != null)
            {
                this.ai.Update();
            }
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            if (this.ai!=null)
            {
                this.ai.OnDamage(damage, source);
            }
        }   
    }
}
