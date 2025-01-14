using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public List<PlayerMovement> players = new List<PlayerMovement>(); // ������ �������
    public int currentPlayerIndex = 0; // ������ �������� ������
    public int totalPlayers = 2; // ���������� ������� (�� 2 �� 4)

    public bool isPlayerMoving = false; // ����, ����� ������������� ����� ������ �� ����� ��������

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DiceController.OnDiceRolled += RollResult;
    }

    private void OnDisable()
    {
        DiceController.OnDiceRolled -= RollResult;
    }

    public void StartPlayerCheck()
    {
        // ��������, ��� ���������� ������� � �������� �� 2 �� 4
        if (totalPlayers < 2 || totalPlayers > 4)
        {
            Debug.LogError("���������� ������� ������ ���� �� 2 �� 4");
            return;
        }

        // ������ ����� ���� ��������� ����� ���������
        if (players.Count != totalPlayers)
        {
            while (players.Count!=totalPlayers)
            {
                players.RemoveAt(players.Count-1);
                Debug.Log("Removed player");
            }
            return;
        }

        // ������ ���� � ������� ������
        
    }

    private void Update()
    {
        // ���������, ����� �� ������� ����� �� ������ ��� ������ ������
        if (!isPlayerMoving && Input.GetKeyDown(KeyCode.J)) // ���� ������ J ������
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            PlayerMovement currentPlayer = players[currentPlayerIndex];
            if (currentPlayer.isSkipTurn)
            {
                currentPlayer.isSkipTurn = false;
                return;
            }
            DiceController.instance.DiceRoll();
        }
    }
    
    public void TurnButton()
    {
        if (!isPlayerMoving)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            PlayerMovement currentPlayer = players[currentPlayerIndex];
            if (currentPlayer.isSkipTurn)
            {
                currentPlayer.isSkipTurn = false;
                UITemplate.instance.ShowSkipTurnMessage();
                EndTurn();
                return;
            }
            DiceController.instance.DiceRoll();
            CameraController.instance.LookAtDice();
        }
    }
    private void RollResult(int result)
    {
        PlayerMovement currentPlayer = players[currentPlayerIndex];
        currentPlayer.RollResult(result);
        CameraController.instance.LookAtCurrentPlayer(currentPlayer.gameObject);
    }

    public void StartTurn()
    {
        if (isPlayerMoving)
            return;
        
        // �������� �������� ������
        PlayerMovement currentPlayer = players[currentPlayerIndex];

        UITemplate.instance.ShowCurrentPlayer(currentPlayerIndex + 1);

        currentPlayer.CorrectPlayerPosition();
    }

    public void EndTurn()
    {
        // ��������� ��� �������� ������
        isPlayerMoving = false;

        StartTurn();
    }

    public void SetPlayerMoving(bool isMoving)
    {
        isPlayerMoving = isMoving; // ������������� ���� ��������
    }
}
