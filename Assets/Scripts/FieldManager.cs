using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance;

    public Cell[] cells; // ������ ���� ������ �� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            cells=GetComponentsInChildren<Cell>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
