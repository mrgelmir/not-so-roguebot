using UnityEngine;
using System.Collections;

/// <summary>
/// Wraps the Unity logging class so that it won't be logged in a build
/// Possible future features : html logging
/// </summary>
public static class Log
{
	public static void Message(Object message, Object context = null)
	{
#if UNITY_EDITOR
		Debug.Log(message, context);
#endif
	}

	public static void Error(Object message, Object context = null)
	{
#if UNITY_EDITOR
		Debug.LogError(message, context);
#endif
	}

	public static void Warning(Object message, Object context = null)
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
