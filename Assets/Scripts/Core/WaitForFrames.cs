using UnityEngine;

public class WaitForFrames : CustomYieldInstruction
{
    private uint m_CurrrentFrame;
    public WaitForFrames(uint frames)
    {
        m_CurrrentFrame = frames;
    }


    public override bool keepWaiting
    {
        get
        {
            return (--m_CurrrentFrame) > 0;
        }
    }
}