namespace TagReplacer.Entities;

using System;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long SubsetId { get; set; }

    public string CreatorId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; }

    [Required]
    [StringLength(8)]
    public string SrtId { get; set; }

    [Required, MaxLength(100)]
    public string ImageName { get; set; }

    public string MinDescription { get; set; }

    public string Description { get; set; }

    [Required, MaxLength(200)]
    public string KeyWords { get; set; }

    [Required]
    public float Weight { get; set; }

    [Required]
    public DateTime CreateDate { get; set; }

    public bool IsBlocked { get; set; }

    [MaxLength(100)]
    public string OldTitle { get; set; }
}
