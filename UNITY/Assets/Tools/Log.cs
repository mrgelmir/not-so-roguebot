// temp editor only (find another way to fix this in the future)
#if UNITY_EDITOR || DEBUG
#define ENABLELOGGING
#endif

using System.Diagnostics;
using Utils;
/// <summary>
/// Wraps the Unity logging class so that it won't be logged in a build
/// Possible future features : html logging
/// </summary>
public static class Log
{

	[Conditional("ENABLELOGGING")]
	public static void Write(object message, UnityEngine.Color32 color, UnityEngine.Object context = null)
	{

		Message("<color=#" + color.ToHex() + ">" + message + "</color>", context);
	}

	[Conditional("ENABLELOGGING")]
	public static void Write(object message, UnityEngine.Object context = null)
	{
		Message(message, context);
	}

	[Conditional("ENABLELOGGING")]
	public static void Message(object message, UnityEngine.Object context = null)
	{
#if UNITY_EDITOR
		UnityEngine.Debug.Log(message, context);
#endif
	}

	[Conditional("ENABLELOGGING")]
	public static void Error(object message, UnityEngine.Object context = null)
	{
#if UNITY_EDITOR
		UnityEngine.Debug.LogError(message, context);
#endif
	}

	[Conditional("ENABLELOGGING")]
	public static void Warning(object message, UnityEngine.Object context = null)
	{
#if UNITY_EDITOR
		UnityEngine.Debug.LogWarning(message, context);
#endif
	}

	[Conditional("ENABLELOGGING")]
	public static void Exception(System.Exception exception, UnityEngine.Object context = null)
	{
#if UNITY_EDITOR
		UnityEngine.Debug.LogException(exception, context);
#endif
	}
}
