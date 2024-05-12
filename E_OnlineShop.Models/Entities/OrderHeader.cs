using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_OnlineShop.Models.Entities
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; } = null!;
		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; } = null!;

		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public double OrderTotal { get; set; }

		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }

		public DateTime PaymentDate { get; set; }
		public DateTime PaymentDueDate { get; set; }

		public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }

		[Required]
		public string PhoneNumber { get; set; } = string.Empty;
		[Required]
		public string StreetAddress { get; set; } = string.Empty;
		[Required]
		public string City { get; set; } = string.Empty;
		[Required]
		public string State { get; set; } = string.Empty;
		[Required]
		public string PostalCode { get; set; } = string.Empty;
		[Required]
		public string Name { get; set; } = string.Empty;


	}
}
