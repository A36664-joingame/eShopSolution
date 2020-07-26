using eShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
   public class Order
    {
        public int Id { set; get; }
        public DateTime OrderDate { set; get; }
        public Guid UserId { set; get; }
        public string ShipName { set; get; }
        public string ShipAddress { set; get; }
        public string ShipEmail { set; get; }
        public string ShipPhoneNumber { set; get; }
        public OrderStatus Status { set; get; }

        public List<OrderDetail> OrderDetails { get; set; }// để cầu hình 1 nhiều với OrderDetail
        // để list bên số ít có nghĩa là một Order có thể thuộc nhiều OrderDetail
        // khi bên số nhiều tham chiếu thì nó sẽ có withMany chứng tỏ đúng

        public AppUser AppUser { get; set; }// để tham chiếu tới AppUser

    }
}
