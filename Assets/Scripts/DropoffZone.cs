using UnityEngine;

//[On pickup zones]
//Holds a nPickups variable
public class DropoffZone : MonoBehaviour
{
    [SerializeField] private int nDropoffs;
    private TextMesh display;

    private void Start()
    {
        display = GetComponentInChildren<TextMesh>();
    }
    public void SetnDropoffs(int n)
    {
        nDropoffs = n;
        display.text = nDropoffs.ToString();
    }
    public int GetState()
    {
        if (nDropoffs != 0)
            return 1;
        else return 0;
    }
}
