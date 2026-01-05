using GestaoClientes.Domain.Exceptions;
using GestaoClientes.Domain.ValueObjects;

namespace GestaoClientes.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string NomeFantasia { get; private set; }
        public Cnpj Cnpj { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        private Cliente()
        {
            NomeFantasia = string.Empty;
            Cnpj = null!;
        }

        private Cliente(string nomeFantasia, Cnpj cnpj)
        {
            Id = Guid.NewGuid();
            DataCadastro = DateTime.UtcNow;
            Ativo = true;

            ValidarEAtribuirNomeFantasia(nomeFantasia);
            Cnpj = cnpj ?? throw new DomainException("O CNPJ não pode ser nulo.");
        }

        public static Cliente Criar(string nomeFantasia, Cnpj cnpj)
        {
            return new Cliente(nomeFantasia, cnpj);
        }

        public void AtualizarNomeFantasia(string novoNomeFantasia)
        {
            ValidarEAtribuirNomeFantasia(novoNomeFantasia);
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Ativar()
        {
            if (Ativo)
                throw new DomainException("O cliente já está ativo.");

            Ativo = true;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Inativar()
        {
            if (!Ativo)
                throw new DomainException("O cliente já está inativo.");

            Ativo = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        private void ValidarEAtribuirNomeFantasia(string nomeFantasia)
        {
            if (string.IsNullOrWhiteSpace(nomeFantasia))
                throw new DomainException("O nome fantasia não pode ser vazio.");

            if (nomeFantasia.Length < 3)
                throw new DomainException("O nome fantasia deve ter pelo menos 3 caracteres.");

            if (nomeFantasia.Length > 200)
                throw new DomainException("O nome fantasia não pode exceder 200 caracteres.");

            NomeFantasia = nomeFantasia.Trim();
        }
        public override string ToString() => $"{NomeFantasia} - {Cnpj.ObterFormatado()}";

    }
}
