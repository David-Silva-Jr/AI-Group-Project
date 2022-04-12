using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentVisualizer : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;

    private float t;
    private float moveTime;

    private List<Tile> map;

    private ALT_TimeSystem timer;

    private DAgent agent;
    public  string agentName;

    public Transform model;

    public Material oldColor;
    public Material newColor;

    // Start is called before the first frame update
    void Awake()
    {
        timer = FindObjectOfType<ALT_TimeSystem>();
        moveTime = timer.GetMaxTickTimer()/2;
        t = 0;

        model = GetComponent<Transform>();

        startPos = model.position;
        targetPos = startPos;

        map = FindObjectOfType<MapGenTest>().Map;

        agent = new DAgent(agentName, map[0]);

        ALT_TimeSystem.Tick += OnTick;
        agent.LocationChanged += OnAgentLocationChanged;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / moveTime;
        transform.position = Vector3.Lerp(startPos, targetPos, t);
    }

    private void OnTick(object sender, ALT_TimeSystem.OnTickEventArgs e){
        agent.DoAction("random_move");
    }

    private void OnAgentLocationChanged(object sender, DAgent.PropertyChangedEventArgs<Tile> e){
        t = 0;
        startPos = targetPos;

        GameObject startCell = GameObject.Find("Cell " + e.oldValue.ID);
        GameObject targetCell = GameObject.Find("Cell " + e.newValue.ID);

        targetPos = targetCell.transform.position + Vector3.up*1.5f;

        startCell.GetComponent<MeshRenderer>().sharedMaterial = oldColor;
        targetCell.GetComponent<MeshRenderer>().sharedMaterial = newColor;
    }

    private void OnDestroy(){
        ALT_TimeSystem.Tick -= OnTick;
    }
}
