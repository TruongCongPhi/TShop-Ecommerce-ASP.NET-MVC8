using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Thumb { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateModified { get; set; }

    public string? Author { get; set; }

    public int? AccountId { get; set; }

    public bool? Published { get; set; }

    public bool IsHot { get; set; }

    public bool IsNewFeed { get; set; }

    public int? Views { get; set; }

    public int? BlogCategoryId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual BlogCategory? BlogCategory { get; set; }

    public virtual ICollection<BlogImage> BlogImages { get; set; } = new List<BlogImage>();
}
