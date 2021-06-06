using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntergration
{
   public interface IOrderApiClient
    {

        Task<int> CreateOrder(CheckoutRequest request);

    }
}
