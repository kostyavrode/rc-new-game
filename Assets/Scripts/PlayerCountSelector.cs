using UnityEngine;
using UnityEngine.UI;
using TMPro; // ��� ����������� ������ � ����������� �������

public class PlayerCountSelector : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider playerCountSlider; // ��������
    public TMP_Text playerCountText; // ����� ��� ����������� ���������� ���������� �������

    private int playerCount = 2; // ��������� ���������� �������

    private void Start()
    {
        // ����������� ��������
        playerCountSlider.minValue = 2;
        playerCountSlider.maxValue = 4;
        playerCountSlider.wholeNumbers = true;
        playerCountSlider.value = playerCount;

        // ��������� ����� � ������� �������
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
