using UnityEngine;
using System.Collections;

public class SpriteAnimation : ScriptableObject
{
    public int startFrame;
    public int endFrame;
    public float duration;
    public WrapMode wrapMode;

    //=======================================

    /// <summary>
    /// return the frame for the giver time, taken in account the wrapMode
    /// </summary>
    /// <param name="time"></param>
    /// <returns>the frame number, -1 if not valid time (passed duration if wrap.once for exemple)</returns>
    public int SampleAt(float time)
    {
        if (endFrame < startFrame)
            endFrame = startFrame;

        float nomalizedTime = time;
        while (nomalizedTime < 0 || nomalizedTime > duration)
        {
            if (wrapMode == WrapMode.Once)
                return -1;

            switch (wrapMode)
            {
                case WrapMode.Clamp:
                case WrapMode.ClampForever:
                    nomalizedTime = Mathf.Clamp(nomalizedTime, 0, duration);
                    break;
                case WrapMode.Default:
                    nomalizedTime = duration;
                    break;
                case WrapMode.Loop:
                    nomalizedTime = nomalizedTime < 0 ? nomalizedTime + duration : nomalizedTime - duration;
                    break;
                case WrapMode.PingPong:
                    nomalizedTime = Mathf.PingPong(nomalizedTime, duration);
                    break;
                default:
                    nomalizedTime = duration;
                    break;
            }
        }

        int frame = (int)(startFrame + (nomalizedTime / duration) * (endFrame+1 - startFrame ));

        return frame;
    }
}