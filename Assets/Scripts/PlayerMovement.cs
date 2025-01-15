using DG.Tweening;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float moveDuration = 0.5f;  // ����� �������� �� ������
    private int currentCellIndex = 0;  // ������ ������� ������

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
        // ��������� ������� ������ � ������� ������
        int targetCellIndex = currentCellIndex + diceRoll;

        // ���������, ��� ������� ������ ����������
        if (targetCellIndex < FieldManager.Instance.cells.Length)
        {
            StartCoroutine(MoveThroughCells(targetCellIndex));  // ��������� �������� ��� �������
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
        // ������ ���������� ������ ����� ������ ������, ���� �� ������ �� ������� ������
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
                    int moveForwardTarget = Mathf.Min(FieldManager.Instance.cells.Length - 1, currentCellIndex + finalCell.moveBackAmount);
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
        targetCellIndex = Mathf.Min(FieldManager.Instance.cells.Length - 1, targetCellIndex); // ������������ �������

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
