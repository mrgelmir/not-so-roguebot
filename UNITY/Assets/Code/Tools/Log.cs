using UnityEngine;
using System.Collections;

/// <summary>
/// Wraps the Unity logging class so that it won't be logged in a build
/// Possible future features : html logging
/// </summary>
public static class Log
{
	public static void Write(object message, Object context = null)
	{
		Message(message, context);
	}

	public static void Message(object message, Object context = null)
	{
#if UNITY_EDITOR
		Debug.Log(message, context);
#endif
	}

	public static void Error(object message, Object context = null)
	{
#if UNITY_EDITOR
		Debug.LogError(message, context);
#endif
	}

	public static void Warning(object message, Object context = null)
	{
#if UNITY_EDITOR
		Debug.LogWarning(message, context);
#endif
	}

	public static void Exception(System.Exception exception, Object context = null)
	{
#if UNITY_EDITOR
		Debug.LogException(exception, context);
#endif
	}
}
