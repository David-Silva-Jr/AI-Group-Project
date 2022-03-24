using System.Collections;
using System;
using UnityEngine;

public class ALT_TimeSystem : MonoBehaviour
{
    [SerializeField] private bool paused;

    public class OnTickEventArgs: EventArgs
    {
        public int tick_number;
    }

    public static event EventHandler<OnTickEventArgs> Tick;

    [SerializeField] private float tick_timer_max = 1f;

    private int tick;
    private float tickTimer;
    
    private void Awake()
    {
        paused = false;
        tick = 0;
    }

    private void Update()
    {
        if(!paused)
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= tick_timer_max)
            {
                tickTimer -= tick_timer_max;
                tick++;
                Tick?.Invoke(this, new OnTickEventArgs { tick_number = tick });
            }
        }
    }

    public float GetMaxTickTimer() //Let agents access the tick timer so they can move at speed relative to the global tick time
    {
        return tick_timer_max;
    }

    public void Pause()
    {
        paused = !paused;
    }
}
