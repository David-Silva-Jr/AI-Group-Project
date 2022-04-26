using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

//[On WorldManager]
//The time system is a tick based system that determine agent movement
public class TimeSystem : MonoBehaviour
{
    [SerializeField] private Text timer;
    [SerializeField] private GameObject stepTimeInputField;

    //Terminal step
    private string terminalSteps = "";

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
                timer.text = "Step: " + (tick/2).ToString();
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

    public void SetStepTimer()
    {
        tick_timer_max = float.Parse(stepTimeInputField.GetComponent<Text>().text);
    }

    public void PrintTime()
    {
        terminalSteps += (tick / 2) + ",";
        Debug.Log(terminalSteps);
    }
}
