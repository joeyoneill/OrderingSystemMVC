using System;
using System.Collections.Generic;

namespace mvc_obj_2.Data;

public partial class Item
{
    public int Id { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal ItemPrice { get; set; }

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();
}
