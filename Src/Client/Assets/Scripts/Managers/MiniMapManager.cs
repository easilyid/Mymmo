using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    class MinimapManager : Singleton<MinimapManager>
    {

        public UIMinimap minimap;
        private Collider minimapBoudingBox;

        public Collider MinimapBoudingBox
        {
            get { return minimapBoudingBox; }
        }
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                    return null;
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoudingBox = minimapBoundingBox;
            if (this.minimap!=null)
                this.minimap.UpdateMap();
            
        }

    }
}
