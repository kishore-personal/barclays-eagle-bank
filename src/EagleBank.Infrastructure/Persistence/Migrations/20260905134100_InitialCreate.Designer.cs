using EagleBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EagleBank.Infrastructure.Persistence.Migrations;

[DbContext(typeof(EagleBankDbContext))]
[Migration("20260905134100_InitialCreate")]
partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.11");
        EagleBankDbContextModelSnapshot.BuildEagleBankModel(modelBuilder);
    }
}
