using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test_BonusWheelController : MonoBehaviour
{
    [SerializeField] BonusWheelController wheel = default;
    private int testCounter = 0;

    public void RunTest()
    {
        EraseOldTestResults();

        PrintTestResults(100);
        PrintTestResults(1000);
        PrintTestResults(10000);
    }

    private void EraseOldTestResults()
    {
        string path = "Assets/Resources/Test_BonusWheelController.txt";
        File.WriteAllText(path, System.String.Format(""));
    }

    private void PrintTestResults(int spinAmmount)
    {
        testCounter++;
        int[] results = wheel.MassWheelSpins(spinAmmount);
        string resultsString = "";

        resultsString += ($"Test {testCounter} - Wheel Spun {spinAmmount} Times\n");

        for (int i = 0; i < results.Length; i++)
        {
            resultsString+=$"Section {wheel.wheelSections[i].Index} - {wheel.wheelSections[i].Name} - Target Weight: {wheel.wheelSections[i].Weight}% - Mass Spin Results {results[i]} ({results[i] / (float)spinAmmount:P})\n";
        }
        resultsString += "\n";

        string path = "Assets/Resources/Test_BonusWheelController.txt";
        File.AppendAllText(path, System.String.Format(resultsString));
        Debug.Log("Mass Results Printed To " + path);
    }
}
