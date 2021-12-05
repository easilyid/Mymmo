using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    public class StoryService : Singleton<StoryService>, IDisposable
    {
        public void Init()
        {
            StoryManager.Instance.Init();
        }

        public StoryService()
        {
            MessageDistributer.Instance.Subscribe<StoryStartResponse>(OnStoryStart);
            MessageDistributer.Instance.Subscribe<StoryEndResponse>(OnStoryEnd);

        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StoryStartResponse>(OnStoryStart);
            MessageDistributer.Instance.Unsubscribe<StoryEndResponse>(OnStoryEnd);

        }
        internal void SendStartStory(int storyId)
        {
            Debug.Log("SendStartStory" + storyId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyStart = new StoryStartRequest();
            message.Request.storyStart.storyId = storyId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnStoryStart(object sender, StoryStartResponse message)
        {
            Debug.Log("SendStartStory" + message.storyId);
            StoryManager.Instance.OnStoryStart(message.storyId);
        }
        internal void SendEndStory(int storyId)
        {
            Debug.Log("SendEndStory" + storyId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyEnd = new StoryEndRequest();
            message.Request.storyStart.storyId = storyId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnStoryEnd(object sender, StoryEndResponse message)
        {
            Debug.Log("OnStoryEnd" + message.storyId);

            if (message.Result == Result.Success)
            {

            }
        }


    }
}
