using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour{
    public int ID;
    Mesh mesh = null;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;

    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (this.mesh!=null)
        {
            Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * 0f, this.transform.rotation, this.transform.localScale);
        }
        UnityEditor.Handles.color=Color.red;
        UnityEditor.Handles.ArrowHandleCap(0,transform.position,transform.rotation,1f,EventType.Repaint);
    }
#endif

    void OnTriggerEnter(Collider other)
    {
        PlayerInputController playerController = other.GetComponent<PlayerInputController>();
        if (playerController!=null&&playerController.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[ID];
            if (td==null)
            {
                Debug.LogErrorFormat("TeleporterObject:Character[{0}]Enter Teleporter[{1}],But TeleporterDefine not existed",playerController.character.Info.Name,this.ID);
                return;
            }
            Debug.LogFormat("TeleporterObject:Character[{0}]Enter Teleporter[{1}:{2}]",playerController.character.Info.Name,td.ID,td.Name);
            if (td.LinkTo>0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                
                    MapService.Instance.SendMapTeleport(this.ID);
                else
                
                    Debug.LogErrorFormat("Teleporter ID:{0} LinkID {1} error!",td.ID,td.LinkTo);
                
            }
        }
    }
}
