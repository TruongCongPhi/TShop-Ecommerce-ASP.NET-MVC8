using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class ProductComment
{
    public int CommentId { get; set; }

    public int? ProductId { get; set; }

    public int? AccountId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateAt { get; set; }

    public bool? Deleted { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Product? Product { get; set; }
}
