using System;
using System.Collections.Generic;

namespace TShop.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Address { get; set; }

    public int? LocationId { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public bool Active { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Location? Location { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual Role? Role { get; set; }
}
