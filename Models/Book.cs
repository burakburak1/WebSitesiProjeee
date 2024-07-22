﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebApiXd.Models;


public partial class Book
{

    public int BookId { get; set; }

    [Required]
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }


    public int SellerId { get; set; }
    [JsonIgnore]
    public User Seller { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
public class BookGetDto
{

    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int SellerId { get; set; }
}

public class BookUpdateDto
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class BookPostDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int SellerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; }

}