using System;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [Serializable]
    public class GrowthStage
    {
        public GameObject stageObject;
        public int daysToNextStage;
    }

    public GrowthStage[] growthStages;
    public int dateCreatedDay;
    public TimeManager.Season dateCreatedSeason;

    private int currentStage = 0;

    private void Start()
    {
        if (dateCreatedDay == 0)
        {
            dateCreatedDay = int.Parse(TimeManager.Instance.GetCurrentDay());
            dateCreatedSeason = TimeManager.Instance.GetCurrentSeason();
        }

        InitializeGrowthStages();

        // Assina o evento de início do dia do TimeManager
        TimeManager.Instance.onDayStart.AddListener(CheckGrowthStage);
    }

    private void InitializeGrowthStages()
    {
        bool stageFound = false;
        
        for (int i = 0; i < growthStages.Length; i++)
        {
            if (growthStages[i].stageObject != null)
            {
                if (growthStages[i].stageObject.activeSelf)
                {
                    currentStage = i;
                    stageFound = true;
                    Debug.Log($"Initial active growth stage found: {currentStage}");
                }
                else
                {
                    growthStages[i].stageObject.SetActive(false);
                }
            }
        }

        if (!stageFound && growthStages.Length > 0 && growthStages[currentStage].stageObject != null)
        {
            growthStages[currentStage].stageObject.SetActive(true);
            Debug.Log($"No active growth stage found. Setting initial stage to: {currentStage}");
        }
    }

    private void CheckGrowthStage()
    {
        int daysPassed = CalculateDaysPassed();
        Debug.Log($"Days passed since creation: {daysPassed}");

        while (currentStage < growthStages.Length - 1 && daysPassed >= growthStages[currentStage].daysToNextStage)
        {
            daysPassed -= growthStages[currentStage].daysToNextStage;
            AdvanceGrowthStage();
        }

        if (currentStage >= growthStages.Length - 1)
        {
            Debug.Log("Reached final growth stage, stopping monitoring.");
            TimeManager.Instance.onDayStart.RemoveListener(CheckGrowthStage);
        }
    }

    private int CalculateDaysPassed()
    {
        int currentDay = int.Parse(TimeManager.Instance.GetCurrentDay());
        TimeManager.Season currentSeason = TimeManager.Instance.GetCurrentSeason();

        int createdSeasonIndex = (int)dateCreatedSeason;
        int currentSeasonIndex = (int)currentSeason;

        int daysPassed = 0;

        if (currentSeasonIndex >= createdSeasonIndex)
        {
            daysPassed = (currentSeasonIndex - createdSeasonIndex) * 30 + (currentDay - dateCreatedDay);
        }
        else
        {
            daysPassed = ((4 - createdSeasonIndex) + currentSeasonIndex) * 30 + (currentDay - dateCreatedDay);
        }

        return daysPassed;
    }

    private void AdvanceGrowthStage()
    {
        if (growthStages[currentStage].stageObject != null)
        {
            growthStages[currentStage].stageObject.SetActive(false);
            Debug.Log($"Deactivating stage: {currentStage}");
        }

        currentStage++;

        if (currentStage < growthStages.Length && growthStages[currentStage].stageObject != null)
        {
            growthStages[currentStage].stageObject.SetActive(true);
            Debug.Log($"Activating stage: {currentStage}");
        }
    }
}
