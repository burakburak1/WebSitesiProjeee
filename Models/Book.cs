using System;
using System.Collections.Generic;

namespace WebApiXd.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Author { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public DateTime? CreatedAt { get; set; }
}
