using UnityEngine;
using DG.Tweening;

public class DiceController : MonoBehaviour
{
    public static System.Action<int> OnDiceRolled; // ������� ��� �������� ���������� ������

    public bool isRolling;

    public static DiceController instance;

    private Vector3[] faceDirections = new Vector3[]
    {
        new Vector3(0, 0, 0),    // ����� 1
        new Vector3(-90, -90, -90),   // ����� 2
        new Vector3(0, -90, -90),   // ����� 3
        new Vector3(0, 90, 90),  // ����� 4
        new Vector3(90, -90, 0),  // ����� 5
        new Vector3(0, -180, -180)    // ����� 6
    };

    private void Start()
    {
        instance = this;
        // ��������� ��������� ��������� ������ �� ����� 1
        transform.rotation = Quaternion.Euler(faceDirections[0]);
    }

    public void DiceRoll()
    {
        int randomFace = Random.Range(0, faceDirections.Length);
        isRolling = true;
        Debug.Log(randomFace+1);

        Vector3 randomRotation = new Vector3(
            Random.Range(360, 720), // ��������� ������� �� X
            Random.Range(360, 720), // ��������� ������� �� Y
            Random.Range(360, 720)  // ��������� ������� �� Z\\


        );

        // �������� �������� � �������������� DoTween
        transform.DORotate(randomRotation, 1f, RotateMode.FastBeyond360)
            .OnComplete(() =>
            {
                // ��������� �������� ��������� �� ��������� �����
                transform.DORotate(faceDirections[randomFace], 0.5f).OnComplete(() =>
                {
                    isRolling = false;
                    OnDiceRolled?.Invoke(randomFace + 1);
                });
            });
    }
}
