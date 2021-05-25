using System;
using System.Collections.Generic;
using System.Text;

namespace eShopeSolution.Utilities.Exceptions
{
 public   class EShopeException : Exception
    {
        public EShopeException()
        {
        }

        public EShopeException(string message)
            : base(message)
        {
        }

        public EShopeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
