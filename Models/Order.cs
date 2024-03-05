using System;
namespace bangAzon.Models
{
	public class Order
	{
		public int id { get; set; }
		public int? customerId { get; set; }
		public int? paymentTypeId { get; set; }
		public bool orderStatus { get; set; }
		public DateTime orderDate { get; set; }
		public int sellerId { get; set; }
        //public bool requiresShipping { get; set; }



       // public ICollection<Product> Products { get; set; }
		// public ICollection<ProductOrder> ProductOrders { get; set; }
	}

    //public class TotalSalesDTO
    //{
    //    public decimal TotalSales { get; set; }
    //}

    //public class TotalSalesThisMonthDTO
    //{
    //    public decimal TotalSalesThisMonth { get; set; }
    //}

    //public class AveragePerItemDTO
    //{
    //    public decimal AveragePerItem { get; set; }
    //}

    //public class TotalInventoryByCategoryDTO
    //{
    //    public int CategoryId { get; set; }
    //    public int TotalInventory { get; set; }
    //}

}

