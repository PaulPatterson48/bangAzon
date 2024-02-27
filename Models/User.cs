using System;
namespace bangAzon.Models
{
	public class User
	{
		public int id { get; set; }
		public string? name { get; set; }
		public string? email { get; set; }
		public bool isSeller { get; set; }
		public int storeId { get; set; }

	}
}

