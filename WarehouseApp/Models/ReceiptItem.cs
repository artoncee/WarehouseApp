using System.ComponentModel.DataAnnotations;
using WarehouseApp.Models;

namespace WarehouseApp.Models
{
    public class ReceiptItem // ресурс в поступлении
    {
        public string Id { get; set; }

        public int ReceiptId { get; set; }
        public Receipt Receipt { get; set; }

        [Required]
        public int ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;

        [Required]
        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        [Range(0.0000001, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}
