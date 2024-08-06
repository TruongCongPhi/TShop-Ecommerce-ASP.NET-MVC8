using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class BlogCategory
{
    public int BlogCategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public string? Thumb { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
