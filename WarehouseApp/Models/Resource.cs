using System.ComponentModel.DataAnnotations;
using static WarehouseApp.Models.Enums;

namespace WarehouseApp.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; } = null!;
        public RecordState State { get; set; } = RecordState.Active;
    }
}
