using GestaoClientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoClientes.Infrastructure.Data
{
    public class ClientesDbContext : DbContext
    {
        public ClientesDbContext(DbContextOptions<ClientesDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesDbContext).Assembly);
        }
    }
}
