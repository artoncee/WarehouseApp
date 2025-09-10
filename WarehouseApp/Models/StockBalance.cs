﻿using System;

namespace WarehouseApp.Models
{
    public class StockBalance //  текущий баланс на складе
    {
        public int Id { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public decimal Quantity { get; set; } = 0m;
    }
}
