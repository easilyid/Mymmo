using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>
    {
        public Dictionary<int, Quest> AllQuests = new Dictionary<int, Quest>();
        private List<NQuestInfo> questInfos;

        private readonly Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests =
            new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> OnQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            AllQuests.Clear();
            this.npcQuests.Clear();
            IniQuests();
        }

        private void IniQuests()
        {
            //初始已有任务
            foreach (var info in questInfos)
            {
                var quest = new Quest(info);
                AllQuests[quest.Info.QuestId] = quest;
            }

            CheckAvailableQuests();
            foreach (var kv in AllQuests)
            {
                AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);

            }

        }

        private void CheckAvailableQuests()
        {
            //初始可用任务
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None &&
                    kv.Value.LimitClass != User.Instance.CurrentCharacterInfo.Class)
                    continue;
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacterInfo.Level)
                    continue;
                if (AllQuests.ContainsKey(kv.Key))
                    continue;
                if (kv.Value.PreQuest > 0)
                {
                    if (AllQuests.TryGetValue(kv.Value.PreQuest, out var preQuest))//获取前置任务
                    {
                        if (preQuest.Info == null)
                            continue;//前置任务未接取
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;//前置任务未完成
                    }
                    else
                        continue;
                }
                var quest = new Quest(kv.Value);
                AllQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!npcQuests.ContainsKey(npcId))
            {
                npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }

            List<Quest> availables;
            List<Quest> complates;
            List<Quest> inComplates;

            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }

            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }

            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.InComplete, out inComplates))
            {
                inComplates = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.InComplete] = inComplates;
            }

            if (quest.Info == null)
            {
                if (npcId == quest.Define.AcceptNPC && !npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }

                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.InComplete].Contains(quest))
                    {
                        npcQuests[npcId][NpcQuestStatus.InComplete].Add(quest);
                    }
                }
            }

        }

        /// <summary>
        /// 获取NPC任务状态
        /// </summary>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            var status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.InComplete].Count > 0)
                {
                    return NpcQuestStatus.InComplete;
                }
            }

            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            var status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.InComplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.InComplete].First());
            }
            return false;
        }

        /// <summary>
        /// 显示任务对话框
        /// </summary>
        private bool ShowQuestDialog(Quest quest)
        {
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                var dialog = UIManager.Instance.Show<UIQuestDialog>();
                dialog.SetQuest(quest);
                dialog.OnClose += OnQuestDialogClose;
                return true;
            }

            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                {
                    MessageBox.Show(quest.Define.DialogIncomplete);
                }
            }

            return true;
        }

        private void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            var dialog = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                //
                //MessageBox.Show(dialog.quest.Define.DialogAccept);

                if (dialog.quest.Info == null)
                {
                    QuestService.Instance.SendQuestAccept(dialog.quest);
                }
                else if (dialog.quest.Info.Status == QuestStatus.Complated)
                {
                    QuestService.Instance.SendQuestSubmit(dialog.quest);
                }
            }
            else if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dialog.quest.Define.DialogDeny);
            }
        }

        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }


        public void OnQuestSubmitted(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }

        private Quest RefreshQuestStatus(NQuestInfo quest)
        {
            npcQuests.Clear();
            Quest result;
            if (AllQuests.ContainsKey(quest.QuestGuid))
            {
                AllQuests[quest.QuestId].Info = quest;
                result = AllQuests[quest.QuestId];
            }
            else
            {
                result = new Quest(quest);
                AllQuests[quest.QuestId] = result;
            }
            CheckAvailableQuests();
            foreach (var kv in AllQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
            OnQuestStatusChanged?.Invoke(result);
            return result;
        }
    }
}
