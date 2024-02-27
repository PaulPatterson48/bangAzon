using System;
namespace bangAzon.Models
{
	public class Order
	{
		public int id { get; set; }
		public int customerId { get; set; }
		public int paymentTypeId { get; set; }
		public bool orderStatus { get; set; }
		public DateTime orderDate { get; set; }

	}
}

