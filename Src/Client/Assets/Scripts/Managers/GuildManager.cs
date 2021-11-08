using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo GuildInfo;

        public NGuildMemberInfo MyMemberInfo;
        public bool HasGuild
        {
            get { return this.GuildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            this.GuildInfo = guild;
            if (guild==null)
            {
                MyMemberInfo = null;
                return;
            }

            foreach (var mem in guild.Members)
            {
                if (mem.characterId==User.Instance.CurrentCharacterInfo.Id)
                {
                    MyMemberInfo = mem;
                    break;
                    
                }
            }
        }

        public void ShowGuild()
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if (result==UIWindow.WindowResult.Yes)
            {//创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if (result==UIWindow.WindowResult.No)
            {//加入公会
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
