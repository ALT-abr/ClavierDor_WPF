using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace clavierdor.Models;

public class Player
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Pouvoir { get; set; } = string.Empty;

    public ICollection<Partie> Parties { get; set; } = new List<Partie>();
}
