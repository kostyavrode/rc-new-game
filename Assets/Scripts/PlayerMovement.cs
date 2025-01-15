using DG.Tweening;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float moveDuration = 0.5f;  // Время движения по клетке
    private int currentCellIndex = 0;  // Индекс текущей клетки

    private bool isMovingBack = false;
    private bool isMovingForward = false;

    public bool isSkipTurn;


    private void Awake()
    {
        instance = this;
    }

    public void MovePlayer(int diceRoll)
    {
        if (isMovingBack)
        {
            return;
        }
        // Считываем текущую клетку и целевую клетку
        int targetCellIndex = currentCellIndex + diceRoll;

        // Проверяем, что целевая клетка существует
        if (targetCellIndex < FieldManager.Instance.cells.Length)
        {
            StartCoroutine(MoveThroughCells(targetCellIndex));  // Запускаем корутину для прыжков
        }
        else
        {
            while (targetCellIndex >= FieldManager.Instance.cells.Length)
            {
                targetCellIndex--;
            }
            StartCoroutine(MoveThroughCells(targetCellIndex));
            UITemplate.instance.EndGame(true);
        }
    }

    public void SkipTurn()
    {
        isSkipTurn = true;
        Debug.Log("Skip Turn");
    }

    private IEnumerator MoveThroughCells(int targetCellIndex)
    {
        // Плавно перемещаем игрока через каждую клетку, пока не дойдем до целевой клетки
        while (currentCellIndex < targetCellIndex)
        {
            Transform currentCell = FieldManager.Instance.cells[currentCellIndex].transform;
            Transform nextCell = FieldManager.Instance.cells[currentCellIndex + 1].transform;

            Vector3 direction = nextCell.position - transform.position;
            direction.y = 0;
            transform.DORotateQuaternion(Quaternion.LookRotation(-direction), moveDuration / 2);

            yield return transform.DOMove(nextCell.position, moveDuration).WaitForCompletion();

            currentCellIndex++;
        }

        while (currentCellIndex > targetCellIndex)
        {
            Transform currentCell = FieldManager.Instance.cells[currentCellIndex].transform;
            Transform previousCell = FieldManager.Instance.cells[currentCellIndex - 1].transform;

            Vector3 direction = previousCell.position - transform.position;
            direction.y = 0;
            transform.DORotateQuaternion(Quaternion.LookRotation(-direction), moveDuration / 2);

            yield return transform.DOMove(previousCell.position, moveDuration).WaitForCompletion();
            //transform.DOLookAt(FieldManager.Instance.cells[currentCellIndex+1].transform.position,0.1f);
            currentCellIndex--;
        }

        // Взаимодействуем с конечной клеткой
        Cell finalCell = FieldManager.Instance.cells[currentCellIndex].GetComponent<Cell>();
        if (finalCell != null)
        {
            switch (finalCell.cellType)
            {
                case Cell.CellType.MoveBack:
                    int moveBackTarget = Mathf.Max(0, currentCellIndex - finalCell.moveBackAmount);
                    if (moveBackTarget != currentCellIndex)
                    {
                        yield return MoveThroughCells(moveBackTarget); // Перемещение назад
                    }
                    break;

                case Cell.CellType.MoveForward:
                    int moveForwardTarget = Mathf.Min(FieldManager.Instance.cells.Length - 1, currentCellIndex + finalCell.moveBackAmount);
                    if (moveForwardTarget != currentCellIndex)
                    {
                        yield return MoveThroughCells(moveForwardTarget); // Перемещение вперед
                    }
                    break;

                case Cell.CellType.Skip:
                    SkipTurn();
                    break;

                case Cell.CellType.ExtraRoll:
                    TurnManager.Instance.SetPlayerMoving(false); // Позволяем игроку бросить кубик снова
                    yield break;
            }
        }

        // Завершаем ход, если клетка не требует дополнительного действия
        TurnManager.Instance.SetPlayerMoving(false);
        TurnManager.Instance.EndTurn();
    }

    public void RollResult(int result)
    {
        TurnManager.Instance.SetPlayerMoving(true);
        MovePlayer(result);
    }

    public void MoveForward(int amount)
    {
        // Отметим, что мы начинаем движение вперед
        isMovingForward = true;

        // Проверяем, не выходит ли игрок за пределы поля
        int targetCellIndex = currentCellIndex + amount;
        targetCellIndex = Mathf.Min(FieldManager.Instance.cells.Length - 1, targetCellIndex); // Ограничиваем пределы

        // Плавно перемещаем игрока вперед
        StartCoroutine(MoveThroughCells(targetCellIndex)); // Запускаем корутину для перемещения вперед

        // После того, как движение вперед завершится, сбросим флаг
        isMovingForward = false;
    }

    public void MoveBack(int amount)
    {
        // Отметим, что мы начинаем движение назад
        isMovingBack = true;

        // Проверяем, не пытается ли игрок перейти за начало поля
        currentCellIndex = Mathf.Max(0, currentCellIndex - amount);

        // Плавно перемещаем игрока обратно
        StartCoroutine(MoveThroughCells(currentCellIndex)); // Запускаем корутину для перемещения назад

        // После того, как движение назад завершится, сбросим флаг
        isMovingBack = false;
    }

    public void CorrectPlayerPosition()
    {
        // Проверяем, на какой клетке должен быть игрок
        Transform targetCell = FieldManager.Instance.cells[currentCellIndex].transform;

        // Если позиция игрока не совпадает с позицией клетки, корректируем
        if (Vector3.Distance(transform.position, targetCell.position) > 0.1f)
        {
            // Плавно перемещаем игрока на нужную клетку
            transform.DOMove(targetCell.position, moveDuration);
        }
    }
}
