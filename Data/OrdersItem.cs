using System;
using System.Collections.Generic;

namespace mvc_obj_2.Data;

public partial class OrdersItem
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ItemId { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Order? Order { get; set; }
}
