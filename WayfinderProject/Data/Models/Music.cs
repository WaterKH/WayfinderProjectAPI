﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{

    [Index(nameof(Name), Name = "Index_MusicName")]
    public class Music
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [AllowNull]
        public string Link { get; set; }

        [AllowNull]
        public virtual ICollection<Scene> Scenes { get; set; }
        [AllowNull]
        public virtual ICollection<Interaction> Interactions { get; set; }
        [AllowNull]
        public virtual ICollection<Trailer> Trailers { get; set; }
    }
}