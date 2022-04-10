﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[On pickup zones]
//Holds a nPickups variable
public class PickupZone : MonoBehaviour
{
    [SerializeField] private int nPickups;
    private TextMesh display;

    private void Start()
    {
        display = GetComponentInChildren<TextMesh>();
    }

    public void setnPickups(int n)
    {
        nPickups = n;
        display.text = nPickups.ToString();
    }
}
