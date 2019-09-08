using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewDebugger : MonoBehaviour
{
    private Brew _brew;

    private readonly RollingQueue<int> _rollingNumbers = new RollingQueue<int>(2);

    private static readonly KeyCode[] KeyCodes = 
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    void Awake()
    {
        _brew = GetComponent<Brew>();
    }

    void Update()
    {
        if (!_brew)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_brew.ToString());
        }

        for (var i = 0; i < KeyCodes.Length; i++)
        {
            if (Input.GetKeyDown(KeyCodes[i]))
            {
                _rollingNumbers.Push(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var rowIndex = _rollingNumbers.Peek(1);
            var columnIndex = _rollingNumbers.Peek();
            Debug.Log($"Stirring clockwise at ({rowIndex},{columnIndex})");
            _brew.StirClockwise(rowIndex, columnIndex);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var rowIndex = _rollingNumbers.Peek(1);
            var columnIndex = _rollingNumbers.Peek();
            Debug.Log($"Stirring counter-clockwise at ({rowIndex},{columnIndex})");
            _brew.StirCounterclockwise(rowIndex, columnIndex);
        }
    }

    private class RollingQueue<T>
    {
        private readonly T[] _rollingArray;

        public RollingQueue(int size)
        {
            _rollingArray = new T[size];
        }

        public void Push(T value)
        {
            for (var i = _rollingArray.Length - 1; i > 0; i--)
            {
                _rollingArray[i] = _rollingArray[i - 1];
            }

            _rollingArray[0] = value;
        }

        public T Peek(int index = 0)
        {
            return _rollingArray[index];
        }
    }
}
