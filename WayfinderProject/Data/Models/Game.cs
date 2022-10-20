﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(Name), Name = "Index_GameName")]
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;


        [AllowNull]
        public virtual ICollection<Scene> Scenes { get; set; }
    }
}