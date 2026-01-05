using GestaoClientes.Domain.Entities;
using GestaoClientes.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoClientes.Infrastructure.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(c => c.NomeFantasia)
                .HasColumnName("NomeFantasia")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.Cnpj)
                .HasColumnName("Cnpj")
                .HasMaxLength(14)
                .IsRequired()
                .HasConversion(
                    cnpj => cnpj.Valor,           
                    valor => Cnpj.Criar(valor))   
                .HasColumnType("char(14)");

            builder.Property(c => c.Ativo)
                .HasColumnName("Ativo")
                .IsRequired();

            builder.Property(c => c.DataCadastro)
                .HasColumnName("DataCadastro")
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(c => c.DataAtualizacao)
                .HasColumnName("DataAtualizacao")
                .HasColumnType("datetime2");

            builder.HasIndex(c => c.Cnpj)
                .IsUnique()
                .HasDatabaseName("IX_Clientes_Cnpj");

            builder.HasIndex(c => c.NomeFantasia)
                .HasDatabaseName("IX_Clientes_NomeFantasia");

            builder.HasIndex(c => c.Ativo)
                .HasDatabaseName("IX_Clientes_Ativo");
        }
    }
}
