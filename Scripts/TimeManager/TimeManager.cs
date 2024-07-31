using System;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour, IDataPersistence
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    public float ticksPerSecond = 1f;

    [Header("Initial Settings")]
    public bool forceStartSettings = false;
    public Season startSeason = Season.Summer;
    public int startDay = 1;
    public StartPeriodOption startPeriod = StartPeriodOption.None;
    public int startHour = 6;
    public int startMinute = 0;

    private float tickTimer;
    private int minute;
    private int hour;
    private int day;
    private Season currentSeason;
    private string currentPeriod;

    [Serializable]
    public class Period
    {
        public string name;
        public int startHour;
        public int endHour;
        public UnityEvent onPeriodStart;
    }

    public Period morningPeriod = new Period { name = "Morning", startHour = 6, endHour = 12 };
    public Period afternoonPeriod = new Period { name = "Afternoon", startHour = 12, endHour = 18 };
    public Period eveningPeriod = new Period { name = "Evening", startHour = 18, endHour = 21 };
    public Period nightPeriod = new Period { name = "Night", startHour = 21, endHour = 6 };

    private Period[] periods;

    [Header("Events")]
    public UnityEvent onDayStart;
    public UnityEvent onSeasonChange;

    public enum StartPeriodOption
    {
        None,
        Morning,
        Afternoon,
        Evening,
        Night
    }

    public enum Season
    {
        Summer,
        Winter,
        Autumn,
        Spring
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TimeManager instance initialized.");
            InitializeTimeManager();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTimeManager()
    {
        periods = new Period[] { morningPeriod, afternoonPeriod, eveningPeriod, nightPeriod };

        if (forceStartSettings)
        {
            day = startDay;
            currentSeason = startSeason;

            switch (startPeriod)
            {
                case StartPeriodOption.Morning:
                    hour = morningPeriod.startHour;
                    minute = 0;
                    break;
                case StartPeriodOption.Afternoon:
                    hour = afternoonPeriod.startHour;
                    minute = 0;
                    break;
                case StartPeriodOption.Evening:
                    hour = eveningPeriod.startHour;
                    minute = 0;
                    break;
                case StartPeriodOption.Night:
                    hour = nightPeriod.startHour;
                    minute = 0;
                    break;
                default:
                    hour = startHour;
                    minute = startMinute;
                    break;
            }
        }
        else
        {
            day = 1;
            hour = 6;
            minute = 0;
            currentSeason = Season.Summer;
        }

        UpdatePeriod();
        onSeasonChange.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Playing)
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= 1.0f / ticksPerSecond)
            {
                tickTimer -= 1.0f / ticksPerSecond;
                UpdateTime();
            }
        }
    }

    private void UpdateTime()
    {
        minute++;
        // Debug.Log($"Time Updated: Day {day}, Hour {hour}, Minute {minute}");

        if (minute >= 60)
        {
            minute = 0;
            hour++;
            if (hour >= 24)
            {
                hour = 0;
                day++;
                CheckSeason();
                onDayStart.Invoke();
            }
            UpdatePeriod();
        }
    }

    private void UpdatePeriod()
    {
        foreach (var period in periods)
        {
            if (IsCurrentHourInPeriod(hour, period.startHour, period.endHour))
            {
                if (currentPeriod != period.name)
                {
                    currentPeriod = period.name;
                    period.onPeriodStart.Invoke();
                }
                break;
            }
        }
    }

    private bool IsCurrentHourInPeriod(int currentHour, int startHour, int endHour)
    {
        if (startHour <= endHour)
        {
            return currentHour >= startHour && currentHour < endHour;
        }
        else
        {
            return currentHour >= startHour || currentHour < endHour;
        }
    }

    private void CheckSeason()
    {
        if (day > 30)
        {
            day = 1;
            currentSeason++;
            if ((int)currentSeason > Enum.GetValues(typeof(Season)).Length - 1)
            {
                currentSeason = 0;
            }
            onSeasonChange.Invoke();
        }
    }

    public int GetCurrentMinute() => minute;
    public int GetCurrentHour() => hour;
    public string GetCurrentDay() => day.ToString("00");
    public Season GetCurrentSeason() => currentSeason;
    public string GetCurrentPeriod() => currentPeriod;

    public void LoadData(GameData data)
    {
        minute = data.currentMinute;
        hour = data.currentHour;
        day = data.currentDay;
        currentSeason = (Season)data.currentSeason;
    }

    public void SaveData(GameData data)
    {
        data.currentMinute = minute;
        data.currentHour = hour;
        data.currentDay = day;
        data.currentSeason = (int)currentSeason;
    }
}
