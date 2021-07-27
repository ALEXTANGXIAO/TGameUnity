using System;
using UnityEngine;

struct LogEvent
{
    public LogLevel Level;
    public string Message;
    public float Time;
}

public enum LogLevel
{
    DEBUG,
    INFO,
    WARNING,
    ERROR
}

class GameLogHandler : ILogHandler
{
    public void Log(LogEvent dLogEvent)
    {
        switch (dLogEvent.Level)
        {
            case LogLevel.DEBUG:
                //Debug.Log(ColorUtil.R(ColorType.QualityBrightRed, dLogEvent.Message));
                Debug.Log(dLogEvent.Message);
                break;
            case LogLevel.ERROR:
                Debug.LogError(dLogEvent.Message);
                break;
            case LogLevel.WARNING:
                Debug.LogWarning(dLogEvent.Message);
                break;
            case LogLevel.INFO:
                Debug.Log(dLogEvent.Message);
                break;
            default:
                Debug.Log(dLogEvent.Message);
                break;
        }
    }
}

interface ILogHandler
{
    void Log(LogEvent logEvent);
}

class TDebug:UnitySingleton<TDebug>
{
    static ILogHandler iLogHandler;


    /// <summary>
    /// 加速判断
    /// </summary>
    private static bool m_levelDebug = true;
    private static bool m_levelInfo = true;
    private static bool m_levelError = true;
    private static bool m_levelWarning = true;
    private static bool m_levelFatal = true;

    public static void SetLogHandler(ILogHandler handler)
    {
        if (handler == null) throw new Exception("log handler is null");
        iLogHandler = handler;
    }

    public static void Info(string message, params object[] args)
    {
        if (m_levelInfo)
        {
            Log(LogLevel.INFO, message, args);
        }
    }

    public static void DEBUG(string message, params object[] args)
    {
        if (m_levelDebug)
        {
            Log(LogLevel.DEBUG, message, args);
        }
    }

    public static void Warning(string message, params object[] args)
    {
        if (m_levelWarning)
        {
            Log(LogLevel.WARNING, message, args);
        }
    }

    public static void Error(string message, params object[] args)
    {
        if (m_levelError)
        {

            message = String.Format("{0}\n{1}", message, Environment.StackTrace);
            Log(LogLevel.ERROR, message, args);
        }
    }

    public static void Log(object info)
    {
        string Message = String.Format("{0}|{1}|{2}|", LogLevel.DEBUG, zero, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + info;

        Debug.LogError(Message);

        EventCenter.Instance.EventTrigger(TipsEvent.Log, Message);
    }

    public static void LogError(object info)
    {
        string Message = String.Format("{0}|{1}|{2}|", LogLevel.ERROR, zero, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + info;

        Debug.LogError(Message);

        //EventCenter.Instance.EventTrigger(TipsEvent.Log, Message);
    }

    private static string zero = "0.000000";

    public static void LogNet(object msg)
    {
        string Message = String.Format("{0}|{1}|{2}|", LogLevel.DEBUG, zero, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + msg;

        Debug.Log(Message);

        EventCenter.Instance.EventTrigger(TipsEvent.Log, Message);
    }

    public static void LogNet(string msg, string args)
    {
        string Message = String.Format("{0}|{1}|{2}|", LogLevel.DEBUG, zero, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + String.Format(msg, args);

        Debug.Log(Message);
    }

    public static void LogNet(string msg, params object[] args)
    {
        string Message = String.Format("{0}|{1}|{2}|", LogLevel.DEBUG, zero, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + String.Format(msg, args);

        Debug.Log(Message);

        EventCenter.Instance.EventTrigger(TipsEvent.Log, Message);
    }

    public static void Log(string msg, string args)
    {
        string Message = String.Format("{0}|{1}|{2}|", LogLevel.DEBUG, Time.realtimeSinceStartup, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + String.Format(msg, args);

        Debug.Log(Message);
    }

    public static void Log( string msg, params object[] args)
    {

        string Message = String.Format("{0}|{1}|{2}|", LogLevel.DEBUG, Time.realtimeSinceStartup, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + String.Format(msg, args);

        Debug.Log(Message);

        EventCenter.Instance.EventTrigger(TipsEvent.Log, Message);
    }

    public static void Log(LogLevel level, string msg, params object[] args)
    {
        if (iLogHandler == null)
        {
            throw new Exception("log handler is null");
        }
        var logEvent = new LogEvent
        {
            Level = level,
            Message =
                String.Format("{0}|{1}|{2}|", level, Time.realtimeSinceStartup, DateTime.Now.ToString("yy-MM-dd hh:mm:ss")) + String.Format(msg, args),
            Time = Time.realtimeSinceStartup
        };

        iLogHandler.Log(logEvent);
    }

    public void Log(LogEvent logEvent)
    {
        switch (logEvent.Level)
        {
            case LogLevel.DEBUG:
                Debug.Log(logEvent.Message);
                break;
            case LogLevel.ERROR:
                Debug.LogError(logEvent.Message);
                break;
            case LogLevel.WARNING:
                Debug.LogWarning(logEvent.Message);
                break;
            case LogLevel.INFO:
                Debug.Log(logEvent.Message);
                break;
            default:
                Debug.Log(logEvent.Message);
                break;
        }
    }
}
