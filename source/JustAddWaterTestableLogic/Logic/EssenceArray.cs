using System;
using System.Collections.Generic;
using System.Text;

namespace JustAddWater.Logic
{
    public class EssenceArray
    {
        public int Columns { get; }
        public int Rows { get; }
        private readonly IEssence[] _array;

        public EssenceArray(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            _array = new IEssence[columns * rows];
        }

        public EssenceArray(int columns, IEssence[] existingArray)
        {
            _array = existingArray;
            Columns = columns;
            Rows = existingArray.Length / columns;
        }

        public void Set(int columnIndex, int rowIndex, IEssence essence)
        {
            _array[rowIndex * Columns + columnIndex] = essence;
        }

        public IEssence Get(int columnIndex, int rowIndex)
        {
            return _array[rowIndex * Columns + columnIndex];
        }

        public static implicit operator IEssence[](EssenceArray essenceArray)
        {
            return essenceArray._array;
        }
    }
}
