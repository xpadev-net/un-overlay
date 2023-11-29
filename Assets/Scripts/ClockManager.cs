using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = GenerateClockText();
    }

    private string GenerateClockText()
    {
        var dt = DateTime.Now;
        return dt.Year.ToString()+"/"+dt.Month.ToString().PadLeft(2,'0')+"/"+dt.Day.ToString().PadLeft(2,'0')+"\n"+dt.Hour.ToString().PadLeft(2,'0')+":"+dt.Minute.ToString().PadLeft(2,'0')+":"+dt.Second.ToString().PadLeft(2,'0');
    }
}
