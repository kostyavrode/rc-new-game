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
    public MeshRenderer meshRenderer;

    private void Awake()
    {
        if (cellType==CellType.Normal)
        {
            meshRenderer.material.color = Color.grey;
        }
        else if (cellType==CellType.MoveBack)
        {
            meshRenderer.material.color = Color.yellow;
        }
        else if (cellType==CellType.ExtraRoll)
        {
            meshRenderer.material.color= Color.green;
        }
        else if (cellType==CellType.MoveForward)
        {
            meshRenderer.material.color = Color.blue;
        }
        else if (cellType==CellType.Skip)
        {
            meshRenderer.material.color=Color.red;
        }
    }
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
