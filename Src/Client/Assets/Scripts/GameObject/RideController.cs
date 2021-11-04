using SkillBridge.Message;
using UnityEngine;

public class RideController : MonoBehaviour
{
    public Transform MountPoint;
    public EntityController Rider;
    public Vector3 Offset;
    private Animator anim;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (MountPoint == null || this.Rider == null)
        {
            return;
        }
        Rider.SetRidePosition(MountPoint.position + MountPoint.TransformDirection(Offset));
    }


    public void SetRider(EntityController rider)
    {
        this.Rider = rider;
    }

   

    public void OnEntityEvent(EntityEvent entitySyncEvent, int param)
    {
        switch (entitySyncEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move",false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;

        }
    }
}
