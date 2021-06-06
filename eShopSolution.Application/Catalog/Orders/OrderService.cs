using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace eShopSolution.Application.Catalog.Orders
{
    public class OrderService : IOrderService
    {

        private readonly EShopDbContext _context;


        public OrderService(EShopDbContext context) 
        {
            _context = context; 

      
        }

        public async Task<int> CreateOrder(CheckoutRequest request)
        {

            var user = _context.Users.Where(x => x.UserName == request.UserName).FirstOrDefault();

            var order = new Order()
            {

                OrderDate = DateTime.Now,
                ShipAddress = request.Address,
                ShipEmail = request.Email,
                ShipName = request.Name,
                ShipPhoneNumber = request.PhoneNumber,
                UserId= user.Id,
                OrderDetails = ListOrderdetails(request.OrderDetails)

            };


                _context.Orders.Add(order);

            var item= await _context.SaveChangesAsync();

            return item;

        }


        private List<OrderDetail> ListOrderdetails(List<OrderDetailVm> orderDetailVms)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            foreach(var item in orderDetailVms)
            {
                list.Add(new OrderDetail()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
                    
            }

            return list;
        }
    }
}
