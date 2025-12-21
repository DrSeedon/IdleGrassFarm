using System;
using UnityEngine;

namespace Seedon
{
    public static class LogHelper
    {
        public static void Log(MonoBehaviour context, string message)
        {
            Debug.Log($"[{context.GetType().Name}] {message}");
        }

        public static void LogWarning(MonoBehaviour context, string message)
        {
            Debug.LogWarning($"[{context.GetType().Name}] {message}");
        }

        public static void LogError(MonoBehaviour context, string message)
        {
            Debug.LogError($"[{context.GetType().Name}] {message}");
        }

        public static void LogException(MonoBehaviour context, Exception exception)
        {
            Debug.LogException(exception, context);
        }

        public static void Log(object context, string message)
        {
            Debug.Log($"[{context.GetType().Name}] {message}");
        }

        public static void LogWarning(object context, string message)
        {
            Debug.LogWarning($"[{context.GetType().Name}] {message}");
        }

        public static void LogError(object context, string message)
        {
            Debug.LogError($"[{context.GetType().Name}] {message}");
        }
    }
}
