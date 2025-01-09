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
