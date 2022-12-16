using Microsoft.EntityFrameworkCore;
using System;
using System.IO;


namespace WpfSqliteApp
{
    public class GameContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public static string RootPath { get => Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName; }
        public string folder = RootPath + @"\Game.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
            => optionsbuilder.UseSqlite($"Data Source={folder}");
    }
}
