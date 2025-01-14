using UnityEngine;
using UnityEngine.UI;
using TMPro; // Для отображения текста с количеством игроков

public class PlayerCountSelector : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider playerCountSlider; // Ползунок
    public TMP_Text playerCountText; // Текст для отображения выбранного количества игроков

    private int playerCount = 2; // Начальное количество игроков

    private void Start()
    {
        // Настраиваем ползунок
        playerCountSlider.minValue = 2;
        playerCountSlider.maxValue = 4;
        playerCountSlider.wholeNumbers = true;
        playerCountSlider.value = playerCount;

        // Обновляем текст и создаем игроков
        UpdatePlayerCount((int)playerCountSlider.value);
        playerCountSlider.onValueChanged.AddListener(value => UpdatePlayerCount((int)value));
    }

    private void UpdatePlayerCount(int count)
    {
        playerCount = count;
        UITemplate.instance.turnManager.totalPlayers = playerCount;
        playerCountText.text = $"Players: {playerCount}";
    }
    public void StartPlayerCheck()
    {
        UITemplate.instance.turnManager.StartPlayerCheck();
    }
}
