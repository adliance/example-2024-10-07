using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Example.Web.Models.Database;

public sealed class Db(DbContextOptions options) : DbContext(options)
{
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Registration>(b =>
        {
            b.HasKey(x => x.Id);
        });
    }
}
