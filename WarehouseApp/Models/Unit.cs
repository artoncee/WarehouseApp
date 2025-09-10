using System.ComponentModel.DataAnnotations;
using static WarehouseApp.Models.Enums;

namespace WarehouseApp.Models
{
    public class Unit // единица измерения
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
        public RecordState State { get; set; } = RecordState.Active;
    }
}
