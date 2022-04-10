using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

//[On WorldManager]
//The time system is a tick based system that determine agent movement
public class TimeSystem : MonoBehaviour
{
    [SerializeField] private Text timer;
    private bool paused = true;

    public class OnTickEventArgs: EventArgs
    {
        public int tick;
    }

    public static event EventHandler<OnTickEventArgs> OnTick;

    [SerializeField] private float tick_timer_max = 1f;

    private int tick;
    private float tickTimer;
    
    private void Awake()
    {
        paused = true;
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
                OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
                timer.text = "Time: " + tick.ToString();
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
