using System;
using System.Collections.Generic;

namespace mvc_obj_2.Data;

public partial class Order
{
    public int Id { get; set; }

    public string OrderName { get; set; } = null!;

    public string OrderAddress { get; set; } = null!;

    public decimal? Subtotal { get; set; }

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();
}
