using System;
namespace bangAzon.Models
{
	public class ProductOrder
	{
		public int id { get; set; }
		public int productId { get; set; }
		public int orderId { get; set; }

		// User virtual to enable loading in Entity Framework Core
		public virtual Order orders { get; set; }
		public virtual Product products { get; set; }
	}
}

