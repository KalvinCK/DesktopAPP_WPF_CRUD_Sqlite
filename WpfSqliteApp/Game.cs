using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WpfSqliteApp
{
    [Table("Games")]
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Developer")]
        public string Developer { get; set; } = string.Empty;

        [Column("ReleaseDate")]
        public DateTime ReleaseDate { get; set; }

    }
}
