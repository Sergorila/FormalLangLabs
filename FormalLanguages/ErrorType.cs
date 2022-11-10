using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormalLanguages
{
    public static class ErrorType
    {
        public static void Error(string message, int position)
        {
            throw new Exception($"{message} в позиции: {position}");
        }
    }
}
