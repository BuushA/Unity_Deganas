using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class customer_Panel : MonoBehaviour
{
    [SerializeField] private TMP_Text tooltip;

    //Values kept for later
    //updated on update_labels() call
    string MemT1 = ""; string MemT2 = ""; string MemT3; string Memvisit = "";

    //replace types with string names
    public void update_labels(int score, string type1, string type2, string type3, int visits)
    {
        string text = "";
        text += "score: " + score.ToString() + "\n";
        MemT1 = type1;
        MemT2 = type2;
        MemT3 = type3;
        text += MemT1 + "\n" + MemT2 + "\n" + MemT3 + "\n";
        Memvisit = "Visited: " + visits.ToString();
        text += Memvisit;
        tooltip.text = text;
    }

    //DO NOT CALL BEFORE update_labels()
    public void updateScore(int score)
    {
        string text = "";
        text += "score: " + score.ToString() + "\n";
        text += MemT1 + "\n" + MemT2 + "\n" + MemT3 + "\n";
        text += Memvisit;
        tooltip.text = text;
    }
}
