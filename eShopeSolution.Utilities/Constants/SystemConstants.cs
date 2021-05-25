using System;
using System.Collections.Generic;
using System.Text;

namespace eShopeSolution.Utilities.Constants
{
    public class SystemConstants
    {
        public const string MainConnectionString = "eShopSolutionDb";

        public class Appsettings
        {
            public const string DefaultLanguageId = "DefaultLanguageId";

            public const string Token = "Token";

            public const string BaseAddress = "BaseAddress";
        }

        public class ProductSettings
        {
            public const int TakeFeaturedProduct = 16;
            public const int TakeLatestProducts = 6;
        }
        public class ProductConstants {
            public const string NA = "N/A";
        }
    }
}