using DG.Tweening;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveDuration = 0.5f;  // ����� �������� �� ������
    private int currentCellIndex = 0;  // ������ ������� ������

    private bool isMovingBack = false;
    private bool isMovingForward = false;

    public bool isSkipTurn;

    public void MovePlayer(int diceRoll)
    {
        if (isMovingBack)
        {
            // ���� ��� ����������� ������������ �����, �� ��������� ��������
            return;
        }
        // ��������� ������� ������ � ������� ������
        int targetCellIndex = currentCellIndex + diceRoll;

        // ���������, ��� ������� ������ ����������
        if (targetCellIndex < FieldManager.Instance.cells.Count)
        {
            StartCoroutine(MoveThroughCells(targetCellIndex));  // ��������� �������� ��� �������
        }
        else
        {
            Debug.Log("����� �� ������� ����!");
        }
    }

    public void SkipTurn()
    {
        isSkipTurn = true;
        Debug.Log("Skip Turn");
    }

    private IEnumerator MoveThroughCells(int targetCellIndex)
    {
        // ������ ���������� ������ ����� ������ ������, ���� �� ������ �� ������� ������
        while (currentCellIndex < targetCellIndex)
        {
            Transform currentCell = FieldManager.Instance.cells[currentCellIndex].transform;
            Transform nextCell = FieldManager.Instance.cells[currentCellIndex + 1].transform;

            Vector3 direction = nextCell.position - transform.position;
            direction.y = 0;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction), moveDuration / 2);

            yield return transform.DOMove(nextCell.position, moveDuration).WaitForCompletion();

            currentCellIndex++;
        }

        while (currentCellIndex > targetCellIndex)
        {
            Transform currentCell = FieldManager.Instance.cells[currentCellIndex].transform;
            Transform previousCell = FieldManager.Instance.cells[currentCellIndex - 1].transform;

            Vector3 direction = previousCell.position - transform.position;
            direction.y = 0;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction), moveDuration / 2);

            yield return transform.DOMove(previousCell.position, moveDuration).WaitForCompletion();

            currentCellIndex--;
        }

        // ��������������� � �������� �������
        Cell finalCell = FieldManager.Instance.cells[currentCellIndex].GetComponent<Cell>();
        if (finalCell != null)
        {
            switch (finalCell.cellType)
            {
                case Cell.CellType.MoveBack:
                    int moveBackTarget = Mathf.Max(0, currentCellIndex - finalCell.moveBackAmount);
                    if (moveBackTarget != currentCellIndex)
                    {
                        yield return MoveThroughCells(moveBackTarget); // ����������� �����
                    }
                    break;

                case Cell.CellType.MoveForward:
                    int moveForwardTarget = Mathf.Min(FieldManager.Instance.cells.Count - 1, currentCellIndex + finalCell.moveBackAmount);
                    if (moveForwardTarget != currentCellIndex)
                    {
                        yield return MoveThroughCells(moveForwardTarget); // ����������� ������
                    }
                    break;

                case Cell.CellType.Skip:
                    SkipTurn();
                    break;

                case Cell.CellType.ExtraRoll:
                    TurnManager.Instance.SetPlayerMoving(false); // ��������� ������ ������� ����� �����
                    yield break;
            }
        }

        // ��������� ���, ���� ������ �� ������� ��������������� ��������
        TurnManager.Instance.SetPlayerMoving(false);
        TurnManager.Instance.EndTurn();
    }



    public void RollDice()
    {
        //if (isSkipTurn)
        //{
        //    TurnManager.Instance.EndTurn();
        //    isSkipTurn = false;
        //}
        //// ������ ��� ������ ������ (��������� ������������� ����� ���������)
        //int diceRoll = Random.Range(1, 7);
        //Debug.Log($"������ ������: {diceRoll}");
        //TurnManager.Instance.SetPlayerMoving(true);
        //MovePlayer(diceRoll); // ����� ������ ������ ���������� ������

        //DiceController.instance.DiceRoll();
    }

    public void RollResult(int result)
    {
        TurnManager.Instance.SetPlayerMoving(true);
        MovePlayer(result);
    }

    public void MoveForward(int amount)
    {
        // �������, ��� �� �������� �������� ������
        isMovingForward = true;

        // ���������, �� ������� �� ����� �� ������� ����
        int targetCellIndex = currentCellIndex + amount;
        targetCellIndex = Mathf.Min(FieldManager.Instance.cells.Count - 1, targetCellIndex); // ������������ �������

        // ������ ���������� ������ ������
        StartCoroutine(MoveThroughCells(targetCellIndex)); // ��������� �������� ��� ����������� ������

        // ����� ����, ��� �������� ������ ����������, ������� ����
        isMovingForward = false;
    }

    public void MoveBack(int amount)
    {
        // �������, ��� �� �������� �������� �����
        isMovingBack = true;

        // ���������, �� �������� �� ����� ������� �� ������ ����
        currentCellIndex = Mathf.Max(0, currentCellIndex - amount);

        // ������ ���������� ������ �������
        StartCoroutine(MoveThroughCells(currentCellIndex)); // ��������� �������� ��� ����������� �����

        // ����� ����, ��� �������� ����� ����������, ������� ����
        isMovingBack = false;
    }

    public void CorrectPlayerPosition()
    {
        // ���������, �� ����� ������ ������ ���� �����
        Transform targetCell = FieldManager.Instance.cells[currentCellIndex].transform;

        // ���� ������� ������ �� ��������� � �������� ������, ������������
        if (Vector3.Distance(transform.position, targetCell.position) > 0.1f)
        {
            // ������ ���������� ������ �� ������ ������
            transform.DOMove(targetCell.position, moveDuration);
        }
    }
}
