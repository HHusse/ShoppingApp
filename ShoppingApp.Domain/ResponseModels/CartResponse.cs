using System;
namespace ShoppingApp.Domain.ResponseModels
{
	public class CartResponse
	{
		public string State { get; set; }
		public dynamic Cart { get; set; }
    }
}

