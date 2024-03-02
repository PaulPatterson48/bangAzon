using System;
namespace bangAzon.Models
{
	public class Product
	{
		public int id { get; set; }
		public string? title { get; set; }
		public string? description { get; set; }
		public int quantity { get; set; } = 0;
		public decimal unitPrice { get; set; } = 0.00M;
		public int categoryId { get; set; }
		public int sellerId { get; set; }
		public int customerId { get; set; }
		public DateTime createDate { get; set; }

		public virtual ProductOrder productOrders { get; set; }
		public ICollection<User> users { get; set; }

		public decimal TotalPrice
		{
			get
			{ 
			
				return unitPrice * quantity;
			}
		}
	}
}

