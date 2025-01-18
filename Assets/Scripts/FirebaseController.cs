using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseController : MonoBehaviour
{

    private int maxPlayers;
    private int minPlayers;
    private string orientation;
    private void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase initialized successfully.");
                ConfigureRemoteConfig();
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    private void ConfigureRemoteConfig()
    {
        // Настройка параметров Remote Config
        var settings = new ConfigSettings
        {
            FetchTimeoutInMilliseconds = 3600000, // 1 час
            MinimumFetchIntervalInMilliseconds = 0 // Без задержек между запросами
        };

        FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(settings).ContinueWithOnMainThread(_ =>
        {
            Debug.Log("Remote Config settings updated.");
        });

        FetchRemoteConfig();
    }

    private void FetchRemoteConfig()
    {
        FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result)
            {
                Debug.Log("Remote Config fetched and activated.");
                Debug.Log("Your parameter: " + FirebaseRemoteConfig.DefaultInstance.GetValue("maxplayers").StringValue);
                int maxPlayers = (int)FirebaseRemoteConfig.DefaultInstance.GetValue("maxplayers").LongValue;
                int minPlayers = (int)FirebaseRemoteConfig.DefaultInstance.GetValue("minplayers").LongValue;
                orientation = FirebaseRemoteConfig.DefaultInstance.GetValue("orientation").StringValue;
        
    }
            else
            {
                Debug.LogError("Failed to fetch Remote Config.");
            }
        });
    }
}
