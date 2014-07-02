using System;
using System.Collections.Generic;
using System.Text;

namespace nQuant
{
    public class QuantizationException : ApplicationException
    {
        public QuantizationException(string message) : base(message)
        {

        }
    }
}
