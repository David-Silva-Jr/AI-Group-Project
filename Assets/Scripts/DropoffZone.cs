using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[On pickup zones]
//Holds a nPickups variable
public class DropoffZone : MonoBehaviour
{
    [SerializeField] private int nDropoffs;

    public void setnDropoffs(int n)
    {
        nDropoffs = n;
    }
}
