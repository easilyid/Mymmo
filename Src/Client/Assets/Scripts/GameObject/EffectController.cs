using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public float lifeTime = 1f;
    private float time = 0;

    private Transform target;
    private EffectType type;
    private Vector3 targetPos;
    private Vector3 startPos;
    private Vector3 offset;

    private void OnEnable()
    {
        if (type != EffectType.Bullet)
        {
            StartCoroutine(Run());
        }
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(this.lifeTime);
        this.gameObject.SetActive(false);
    }

    internal void Init(EffectType type, Transform soure, Transform target, Vector3 offset,float duration)
    {
        this.type = type;   
        this.target = target;
        if(duration>0)
            this.lifeTime = duration;
        this.time = 0;
        if (type == EffectType.Bullet)
        {
            this.startPos = this.transform.position;
            this.offset = offset;
            this.targetPos = target.position + offset;
        }
        else if(type==EffectType.Hit)
        {
            this.transform.position = transform.position + offset;
        }
    }

    private void Update()
    {
        if (type == EffectType.Bullet)
        {
            this.time += Time.deltaTime;
            if (this.target != null)
            {
                this.targetPos = this.target.position + this.offset;
            }

            this.transform.LookAt(this.targetPos);
            if (Vector3.Distance(this.targetPos, this.transform.position) < 0.5f)
            {
                Destroy(this.gameObject);
                return;
            }

            if (this.lifeTime>0&&this.time>=this.lifeTime)
            {
                Destroy(this.gameObject);
                return;
            }
            this.transform.position = Vector3.Lerp(this.transform.position, this.targetPos, Time.deltaTime / (this.lifeTime - this.time));
        }
    }


}
