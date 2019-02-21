using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// a set of extension methods meant help with common coroutine cases. Example :
/// <code>
/// void OnTriggerEnter(Collider col) {
///     if(col.gameObject.tag != "Ice")
///             return;
///     Freeze();
///     this.ExecuteLater(()=> Unfreeze(), 2f); // unfreezes the current gameObject 2 seconds from now.
/// }
///
/// </code>
/// </summary>

public static class TimingExtension
{
    public delegate bool When();
    /// <summary>
    /// Execute the given Action when <code>condition</code> returns <code>true</code>.
    /// condition will be evaluated every frame.
    /// </summary>
    /// <param name="action">the action to execute</param>
    /// <param name="condition">Condition.</param>
    public static void ExecuteWhen(this MonoBehaviour m, Action action, When condition)
    {
        m.StartCoroutine(ExecuteWhenCoroutine(action, condition));
    }

    /// <summary>
    /// Execute the action after a delay of <code>seconds</code>
    /// </summary>
    /// <param name="action">Action.</param>
    /// <param name="seconds">Seconds.</param>
    public static void ExecuteLater(this MonoBehaviour m, Action action, float seconds)
    {
        m.StartCoroutine(ExecuteLaterCoroutine(action, seconds));
    }

    /// <summary>
    /// Execute an action next frame
    /// </summary>
    /// <param name="m">M.</param>
    /// <param name="action">Action.</param>
    public static void ExecuteNextFrame(this MonoBehaviour m, Action action)
    {
        m.StartCoroutine(ExecuteNextFrameCoroutine(action));
    }
    private static IEnumerator ExecuteWhenCoroutine(Action action, When condition)
    {
        while (!condition())
            yield return null;
        action();
    }
    private static IEnumerator ExecuteLaterCoroutine(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
    private static IEnumerator ExecuteNextFrameCoroutine(Action action)
    {
        yield return null;
        action();
    }
    public static void Co(this MonoBehaviour m, Func<IEnumerator> coroutine)
    {
        m.StartCoroutine(CoCoroutine(coroutine));
    }
    private static IEnumerator CoCoroutine(Func<IEnumerator> coroutine)
    {
        yield return coroutine;
    }
}