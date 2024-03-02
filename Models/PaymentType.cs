using System;
namespace bangAzon.Models
{
	public class PaymentType
	{
		public int id { get; set; }
		public string name { get; set; }
		public virtual User users { get; set; }

	}
}

