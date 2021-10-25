using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
    public int ID;
    private Mesh mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position + Vector3.up * transform.localScale.y * .5f;
        Gizmos.color = Color.red;
        if (mesh!=null)
        {
            Gizmos.DrawWireMesh(mesh,pos,transform.rotation,transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0,pos,transform.rotation,1f,EventType.Repaint);
        UnityEditor.Handles.Label(pos,"SpawnPoint:"+ID);
    }

#endif


}
