using UnityEngine;
using UnityEngine.AI;

class NavPathRenderer:MonoSingleton<NavPathRenderer>
{
    LineRenderer pathRenderer;
    NavMeshPath path;

    protected override void OnStart()
    {
        //获取组件
        pathRenderer = this.GetComponent<LineRenderer>();
        //默认关闭
        pathRenderer.enabled = false;
    }

    //设置路径渲染
    public void SetPath(NavMeshPath path, Vector3 target)
    {
        //为寻路路径赋值
        this.path = path;
        //如果路径为空
        if(this.path == null)
        {
            //则停止路径渲染
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        }
        else
        {
            //如果路径不为空，开启路径渲染组件
            pathRenderer.enabled = true;
            //设置路径点数量,corners为拐点，终点不包含在内，所以+1
            pathRenderer.positionCount = path.corners.Length + 1;
            //设置每一个路径点位置
            pathRenderer.SetPositions(path.corners);
            //设置目标点位置
            pathRenderer.SetPosition(pathRenderer.positionCount - 1, target);
            for (int i = 0; i < pathRenderer.positionCount; i++)
            {
                //把所有的点向上偏移，防止穿模
                pathRenderer.SetPosition(i, pathRenderer.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}
