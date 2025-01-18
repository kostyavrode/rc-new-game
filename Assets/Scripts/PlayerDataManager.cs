using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private DatabaseReference databaseReference;

    public PlayerDataManager playerDataManager;


    private void SavePlayer()
    {
        string userId = "user123"; // ”никальный идентификатор пользовател€
        string playerName = "Player1";
        int playerScore = 100;

        playerDataManager.SavePlayerData(userId, playerName, playerScore);
        Debug.Log("Player Saved");
    }

    private void Awake()
    {
        InitializeFirebase();
    }
    private void Start()
    {
        SavePlayer();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase initialized successfully.");
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    public void SavePlayerData(string userId, string playerName, int playerScore)
    {
        PlayerData playerData = new PlayerData(playerName, playerScore);
        string json = JsonUtility.ToJson(playerData);

        databaseReference.Child("players").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Player data saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save player data: " + task.Exception);
            }
        });
    }
    public void LoadPlayerData(string userId)
    {
        databaseReference.Child("players").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

                Debug.Log($"Player Name: {playerData.playerName}, Score: {playerData.playerScore}");
            }
            else
            {
                Debug.LogError("Failed to load player data: " + task.Exception);
            }
        });
    }

}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int playerScore;

    public PlayerData(string name, int score)
    {
        playerName = name;
        playerScore = score;
    }
}
