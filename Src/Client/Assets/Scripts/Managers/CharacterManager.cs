using Entities;
using Models;
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

        }

        public void Init()
        {

        }

        public void Clear()
        {
            var keys = Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        public void AddCharacter(Character character)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", character.Id, character.Name, character.Info.mapId, character.Info.Entity.String());
            this.Characters[character.entityId] = character;
            EntityManager.Instance.AddEntity(character);
            OnCharacterEnter?.Invoke(character);
        }

        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);
            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);

                OnCharacterLeave?.Invoke(this.Characters[entityId]);

                this.Characters.Remove(entityId);
            }
        }

        public Character GetCharacter(int id)
        {
            this.Characters.TryGetValue(id, out var character);
            return character;
        }
    }
}
