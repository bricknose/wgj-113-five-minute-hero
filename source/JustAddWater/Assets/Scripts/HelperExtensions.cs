using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class HelperExtensions
    {
        public static int[] To(this int start, int end)
        {
            var returnRange = new int[end - start + 1];
            for (var i = 0; i < returnRange.Length; i++)
            {
                returnRange[i] = start + i;
            }

            return returnRange;
        }
    }
}
