using log4net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityLogger 
{
    public static void Init()
    {
        Application.logMessageReceived += onLogMessageReceived;
    }

    private static ILog log = LogManager.GetLogger("Unity");

    /// <summary>
    /// 读取日志文件时
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="stackTrace">堆栈追踪</param>
    /// <param name="type">类型</param>
    private static void onLogMessageReceived(string condition,string stackTrace, LogType type )
    {
        switch (type)
        {
            case LogType.Error:
                log.ErrorFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Assert:
                log.DebugFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{0\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;
        }
    }
}
