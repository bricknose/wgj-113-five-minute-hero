using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;

public class Brew : MonoBehaviour
{
    [SerializeField]
    private Essence[] _contents;

    // Debugging

    public Brew()
    {
        _contents = new Essence[25];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StirClockwise(int rowIndex, int columnIndex)
    {
        var startingEssence = GetEssence(rowIndex, columnIndex);
        SetEssence(rowIndex, columnIndex, GetEssence(rowIndex + 1, columnIndex));
        SetEssence(rowIndex + 1, columnIndex, GetEssence(rowIndex + 1, columnIndex + 1));
        SetEssence(rowIndex + 1, columnIndex + 1, GetEssence(rowIndex, columnIndex + 1));
        SetEssence(rowIndex, columnIndex + 1, startingEssence);
    }

    public void StirCounterclockwise(int rowIndex, int columnIndex)
    {
        var startingEssence = GetEssence(rowIndex, columnIndex);
        SetEssence(rowIndex, columnIndex, GetEssence(rowIndex, columnIndex + 1));
        SetEssence(rowIndex, columnIndex + 1, GetEssence(rowIndex + 1, columnIndex + 1));
        SetEssence(rowIndex + 1, columnIndex + 1, GetEssence(rowIndex + 1, columnIndex));
        SetEssence(rowIndex + 1, columnIndex, startingEssence);
    }

    private Essence GetEssence(int rowIndex, int columnIndex)
    {
        return _contents[rowIndex * 5 + columnIndex];
    }

    private void SetEssence(int rowIndex, int columnIndex, Essence essence)
    {
        _contents[rowIndex * 5 + columnIndex] = essence;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(Environment.NewLine);
        for (var row = 0; row < 5; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                var essence = GetEssence(row, column);
                stringBuilder.Append(GetEssenceString(essence));
            }

            stringBuilder.Append(Environment.NewLine);
        }

        return stringBuilder.ToString();
    }

    private static string GetEssenceString(Essence essence)
    {
        return essence == null ? $"[  :  ]" : $"[{essence}]";
    }
}
