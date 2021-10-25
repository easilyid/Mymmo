using Common.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapTools : MonoBehaviour
{

    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene{0} not existed", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0}中配置的Teleporter:[{1}]中不存在", map.Value.Resource, teleporter.ID), "确定");
                    return;
                }
                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0}中配置的Teliporter:[{1}]", map.Value.Resource, teleporter.ID), "确定");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");

    }

    //刷怪点逻辑
    [MenuItem("Map Tools/Export SpawnPoints")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();
        //纪录当前场景
        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        if (DataManager.Instance.SpawnPoints == null)
        {
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        }
        //遍历所有地图，并打开
        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("场景 {0} 不存在", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            //获取所有刷怪点
            SpawnPoint[] SpawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }

            foreach (var sp in SpawnPoints)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }
                //定稿至配置文件
                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点导出完成", "确定");
    }

    [MenuItem("Map Tools/Generate NavData")]
    public static void GenerateNavData()
    {
        //动态创建红色材质
        Material red = new Material(Shader.Find("Particles/Alpha Blended")) {color = Color.red};
        red.SetColor("_TintColor", Color.red);
        red.enableInstancing = true;
        //地图包围盒
        GameObject go = GameObject.Find("MapBox");
        if (go != null)
        {
            //地图根节点
            GameObject root = new GameObject("MapRoot");
            //获取包围盒碰撞器
            BoxCollider bound = go.GetComponent<BoxCollider>();
            //设置格子大小
            float step = 1f;
            //从包围盒向上五米，生成三维坐标
            for (float x = bound.bounds.min.x; x < bound.bounds.max.x; x += step)
            {
                for (float z = bound.bounds.min.z; z < bound.bounds.max.y + 5f; z += step)
                {
                    for (float y = bound.bounds.max.y; y < bound.bounds.max.y + 5f; y += step)
                    {
                        var pos = new Vector3(x, y, z);
                        NavMeshHit hit;
                        //位置采样,pos:坐标 out hit:输出为hit,0.5:采样半径,NavMesh:区域
                        if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                        {
                            //如果pos该位置半径0.5内有导航网格，就返回hit
                            if (hit.hit)
                            {
                                //动态创建方块
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                //名称
                                box.name = "Hit" + hit.mask;
                                //颜色
                                box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                //父物体
                                box.transform.SetParent(root.transform, true);
                                //位置
                                box.transform.position = pos;
                                //大小
                                box.transform.localScale = Vector3.one * 0.9f;
                            }
                        }
                    }
                }
            }
        }
    }
}
