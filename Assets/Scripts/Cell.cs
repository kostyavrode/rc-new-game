using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum CellType
    {
        Normal,
        MoveBack,
        ExtraRoll,
        MoveForward,
        Skip
    }

    public CellType cellType;  // ��� ������
    public int moveBackAmount = 5;  // ������� ������ ����� ����������� ������ (��� ���� MoveBack)

    // �����, ������� ����� ����������, ����� ����� ������� �� ������
    public void OnPlayerLanded(PlayerMovement player)
    {
        switch (cellType)
        {
            case CellType.Normal:
                // ������� ������ � ������ �� ����������
                break;
            case CellType.MoveBack:
                //player.MoveBack(moveBackAmount); // ����������� ������ �����
                break;
            case CellType.ExtraRoll:
                //player.RollDice(); // ���������� ������ ������� ��� ���� ������ ������
                break;
            case CellType.MoveForward:
                //player.MoveForward(moveBackAmount); // ���������� ������ ������� ��� ���� ������ ������
                break;
            case CellType.Skip:
                Debug.Log(player.gameObject.name);
                //player.SkipTurn(); // ���������� ������ ������� ��� ���� ������ ������
                break;
        }
    }
}
