using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;


        public CharacterManager()
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public void AddCharacter(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;
            EntityManager.Instance.AddEntity(character);
            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }
        }

        public void RemoveCharacter(int characrerId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characrerId);
            if (this.Characters.ContainsKey(characrerId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[characrerId].Info.Entity);
                if (OnCharacterLeave!=null)
                {
                    OnCharacterLeave(this.Characters[characrerId]);
                }

                this.Characters.Remove(characrerId);
            }
        }
    }
}
