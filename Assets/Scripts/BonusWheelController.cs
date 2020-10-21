using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BonusWheelController : MonoBehaviour
{
    [Header("Wheel Info")]
    [SerializeField] GameObject wheel = default;
    public List<SectionInformation> wheelSections = new List<SectionInformation>();
    private readonly List<int> odds = new List<int>();

    [Header("Animation Settings")]
    public int minSpinRevolutions = 3;
    public int maxSpinRevolutions = 4;
    public float spinDurationInSecionds = 5f;
    public AnimationCurve spinEase = default;
    private bool isSpinning;

    [Header("Results Display")]
    [SerializeField] TextMeshProUGUI resultsText = default;
    [SerializeField] TextMeshProUGUI resultsBySectionText = default;
    private List<int> resultsBySection = new List<int>();

    [Header("Testing")]
    public bool isTestSpin = false;
    public int testSpinWinningSection = 0;


    private void Start()
    {
        isSpinning = false;
        IntializeResultsDisplayBySection();
        RandomizeWheelPosition();
        ConfigureWheelOdds();
    }

    #region Math
    private void ConfigureWheelOdds()
    {
        foreach (SectionInformation section in wheelSections)
        {
            for (int i = 0; i < section.Weight; i++)
            {
                odds.Add(section.Index);
            }
        }
    }

    private void RandomizeWheelPosition()
    {
        //Randomized The Start For Some Variety
        wheel.transform.localEulerAngles = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(0f, 360f));
    }

    public int GetRandomizedWheelSectionAfterOdds()
    {

        int randomIndex = UnityEngine.Random.Range(0, odds.Count);

        //In Test Mode the winning section overides the random winning section.
        if (isTestSpin && testSpinWinningSection < wheelSections.Count)
        {
            Debug.Log($"Test Mode Activated, New Winning Section Is {testSpinWinningSection}");
            return testSpinWinningSection;
        }
        else if (isTestSpin && testSpinWinningSection >= wheelSections.Count)
        {
            Debug.LogError($"Test Mode Activated, New Winning Section Is Out Of Sections Index");
            return odds[randomIndex];
        }
        else
        {
            return odds[randomIndex];
        }
    }

    private float GetSectionAngle(int sectionNumber)
    {
        float angleTotal = 0f;

        //add up the angles before our winning section
        for (int i = 0; i < sectionNumber; i++)
        {
            angleTotal+=wheelSections[i].Angle;
        }

        //add half of our winning angle so we land in the middle you can add variance here
        angleTotal += wheelSections[sectionNumber].Angle / 2f;

        return -angleTotal;
    }

    private float GetCurrentAngleUnder360(float currentWheelAngle)
    {
        while (currentWheelAngle >= 360)
        {
            currentWheelAngle -= 360;
        }
        while (currentWheelAngle < 0)
        {
            currentWheelAngle += 360;
        }

        return currentWheelAngle;
    }
    #endregion

    #region Wheel Spin
    public void StartWheelSpin()
    {
        //Make sure once the user presses the button they cant press again till we are done
        if (isSpinning) return;

        int randomSpinRevolutions = UnityEngine.Random.Range(minSpinRevolutions, maxSpinRevolutions + 1);
        //The Final Result Or Landing Section Of The Wheel
        int randomSectionIndex = GetRandomizedWheelSectionAfterOdds();
        //Math For The Angle Of Where The Wheel Is And How Much It Needs To Turn To Land On Our Final Result
        float currentWheelAngle = GetCurrentAngleUnder360(wheel.transform.eulerAngles.z);
        float sectionAngle = GetSectionAngle(randomSectionIndex);
        float targetAngle = -(sectionAngle + 360f * randomSpinRevolutions);

        Debug.Log($"Wheel spin {randomSpinRevolutions } times before ending at section {randomSectionIndex} with an angle of {sectionAngle}", this);
        Debug.Log($"The odds for this were {wheelSections[randomSectionIndex].Weight / 100f:P} !");
        Debug.Log($"Prize {wheelSections[randomSectionIndex].Name } Won");

        //Counter For Display Region
        resultsBySection[randomSectionIndex]++;
        StartCoroutine(SpinTheWheel(currentWheelAngle, targetAngle, spinDurationInSecionds, wheelSections[randomSectionIndex].Name));
    }

    private IEnumerator SpinTheWheel(float fromAngle, float toAngle, float withinSeconds, string prize)
    {
        isSpinning = true;

        for (float t = 0; t < withinSeconds; t+=Time.deltaTime)
        {
            //Rotate Z Overtime
            float zAngle = Mathf.Lerp(fromAngle, toAngle, spinEase.Evaluate(t / withinSeconds));
            wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x, wheel.transform.localEulerAngles.y, zAngle);
            yield return null;
        }

        wheel.transform.localEulerAngles = new Vector3(wheel.transform.localEulerAngles.x, wheel.transform.localEulerAngles.y, toAngle);
        isSpinning = false;
        UpdateResultsDisplay(prize);
        Debug.Log("Prize wheel done moving");
    }
    #endregion

    #region Display

    private void UpdateResultsDisplay(string prize)
    {
        //just made this region for flavor and fun display the results of the wheel over time

        resultsText.text = prize;

        string newResults = "";
        for (int i = 0; i < resultsBySection.Count; i++)
        {
            newResults += resultsBySection[i].ToString() + " ";
        }
        newResults.Trim();
        resultsBySectionText.text = newResults;
    }

    private void IntializeResultsDisplayBySection()
    {
        for (int i = 0; i < wheelSections.Count; i++)
        {
            resultsBySection.Add(0);
        }
    }

    #endregion

    #region Unit Testing

    public int[] MassWheelSpins(int spinAmmount)
    {
        //make sure test spin is off
        isTestSpin = false;

        //make an array and track the results of the GetRandomizedWheelSectionAfterOdds function
        int[] massTestResultsBySection = new int[wheelSections.Count];
        for (int i = 0; i < spinAmmount; i++)
        {
            massTestResultsBySection[GetRandomizedWheelSectionAfterOdds()]++;
        }

        return massTestResultsBySection;
    }

    #endregion
}
