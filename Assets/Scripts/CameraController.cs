using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform startPosition;
    public Transform dicePosition;

    public GameObject diceObject;
    public GameObject startObject;



    public float lookAtDuration = 0.5f;
    private Transform currentTarget;
    private Tweener lookAtTween;
    private Tweener moveTween; // ���� ��� �����������
    public Transform worldPos; // �����, � ������� ������������ ������ � ��������� �����
    public float moveToWorldPosDuration = 1f; // ����� ����������� � worldPos



    private void Awake()
    {
        instance = this;
    }

    public void GameStartedMove()
    {
        transform.DOMove(startPosition.position, 1);
        transform.DOLookAt(startPosition.position, 0.5f);
    }
    
    public void LookAtDice()
    {
        //transform.DOMove(dicePosition.position, 1);
        //transform.DOLookAt(dicePosition.localPosition, 0.5f);
    }
    public void LookAtCurrentPlayer(GameObject currentPlayer)
    {
        transform.DOMove((transform.position+currentPlayer.transform.position)/2, 1);
        //transform.DOLookAt(currentPlayer.transform.position, 1);
    }
    private void Update()
    {
        // ���������� ������� ����
        if (DiceController.instance.isRolling)
        {
            SetTarget(diceObject.transform);
        }
        else if (TurnManager.Instance.isPlayerMoving)
        {
            SetTarget(TurnManager.Instance.players[TurnManager.Instance.currentPlayerIndex].transform);
        }
        else
        {
            MoveToWorldPos();
        }

        // ������ ������ �� ������� �����, ���� ��� ����
        if (currentTarget != null)
        {
            Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * 5f);
        }
    }

    private void SetTarget(Transform target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;

            // ��������� ���������� ����, ���� �� ��� �� ��������
            lookAtTween?.Kill();

            // ������� ������������ � ����� ����
            lookAtTween = transform.DOLookAt(currentTarget.position, lookAtDuration).SetEase(Ease.InOutQuad);
        }
    }

    private void MoveToWorldPos()
    {
        if (currentTarget != null)
        {
            currentTarget = null; // ������� ����, ���� ����
        }

        // ��������� ���������� ���� �����������, ���� �� ��� �� ��������
        moveTween?.Kill();

        // ������� ����������� � ����� worldPos
        moveTween = transform.DOMove(worldPos.position, moveToWorldPosDuration).SetEase(Ease.InOutQuad);
    }
}
