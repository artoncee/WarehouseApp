using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models
{
    public class Receipt // документ поступления
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Number { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        public List<ReceiptItem> Items { get; set; } = new();       
    }
}
