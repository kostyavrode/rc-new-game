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

    public CellType cellType;  // Тип клетки
    public int moveBackAmount = 5;  // Сколько клеток назад отбрасывает клетка (для типа MoveBack)
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
    // Метод, который будет вызываться, когда игрок попадет на клетку
    public void OnPlayerLanded(PlayerMovement player)
    {
        switch (cellType)
        {
            case CellType.Normal:
                // Обычная клетка — ничего не происходит
                break;
            case CellType.MoveBack:
                //player.MoveBack(moveBackAmount); // Отбрасываем игрока назад
                break;
            case CellType.ExtraRoll:
                //player.RollDice(); // Заставляем игрока сделать еще один бросок кубика
                break;
            case CellType.MoveForward:
                //player.MoveForward(moveBackAmount); // Заставляем игрока сделать еще один бросок кубика
                break;
            case CellType.Skip:
                Debug.Log(player.gameObject.name);
                //player.SkipTurn(); // Заставляем игрока сделать еще один бросок кубика
                break;
        }
    }
}
