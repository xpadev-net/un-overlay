using System;
using TMPro;
using UnityEngine;

public class ClockManager : MonoBehaviour {
    public TextMeshProUGUI text;

    // Update is called once per frame
    private void Update() {
        text.text = GenerateClockText();
    }

    private static string GenerateClockText() {
        var dt = DateTime.Now;
        return dt.Year.ToString() + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" +
               dt.Day.ToString().PadLeft(2, '0') + "\n" + dt.Hour.ToString().PadLeft(2, '0') + ":" +
               dt.Minute.ToString().PadLeft(2, '0') + ":" + dt.Second.ToString().PadLeft(2, '0');
    }
}
