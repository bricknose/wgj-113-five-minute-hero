using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;

public class CauldronBrew : MonoBehaviour
{
    [SerializeField]
    private HeroEssence[] _contents;

    public CauldronBrew()
    {
        _contents = new HeroEssence[25];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebug();
    }

    private void UpdateDebug()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(ToString());
        }

#endif
    }

    private HeroEssence GetEssence(int rowIndex, int columnIndex)
    {
        return _contents[rowIndex * 5 + columnIndex];
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

    private static string GetEssenceString(HeroEssence essence)
    {
        return essence == null ? $"[  :  ]" : $"[{essence}]";
    }
}
