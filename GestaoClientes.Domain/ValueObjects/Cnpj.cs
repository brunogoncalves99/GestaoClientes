using GestaoClientes.Domain.Exceptions;

namespace GestaoClientes.Domain.ValueObjects
{
    public class Cnpj : IEquatable<Cnpj>
    {
        public string Valor { get; }

        private Cnpj(string valor)
        {
            Valor = valor;
        }

        public static Cnpj Criar(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new DomainException("O CNPJ não pode ser vazio.");

            var cnpjNumeros = RemoverFormatacao(cnpj);

            if (cnpjNumeros.Length != 14)
                throw new DomainException("O CNPJ deve conter 14 dígitos.");

            if (TodosDigitosIguais(cnpjNumeros))
                throw new DomainException("O CNPJ não pode ter todos os dígitos iguais.");

            if (!ValidarDigitosVerificadores(cnpjNumeros))
                throw new DomainException("O CNPJ informado é inválido.");

            return new Cnpj(cnpjNumeros);
        }

        private static string RemoverFormatacao(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        private static bool TodosDigitosIguais(string cnpj)
        {
            return cnpj.All(c => c == cnpj[0]);
        }

        private static bool ValidarDigitosVerificadores(string cnpj)
        {
            var digitos = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            var soma = 0;
            var multiplicadores = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 12; i++)
            {
                soma += digitos[i] * multiplicadores[i];
            }

            var resto = soma % 11;
            var primeiroDigito = resto < 2 ? 0 : 11 - resto;

            if (digitos[12] != primeiroDigito)
                return false;

            soma = 0;
            multiplicadores = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 13; i++)
            {
                soma += digitos[i] * multiplicadores[i];
            }

            resto = soma % 11;
            var segundoDigito = resto < 2 ? 0 : 11 - resto;

            return digitos[13] == segundoDigito;
        }

        public string ObterFormatado()
        {
            return $"{Valor.Substring(0, 2)}.{Valor.Substring(2, 3)}.{Valor.Substring(5, 3)}/{Valor.Substring(8, 4)}-{Valor.Substring(12, 2)}";
        }

        public override string ToString() => Valor;

        public override bool Equals(object? obj) => Equals(obj as Cnpj);

        public bool Equals(Cnpj? other)
        {
            if (other is null) return false;
            return Valor == other.Valor;
        }

        public override int GetHashCode() => Valor.GetHashCode();

        public static bool operator ==(Cnpj? left, Cnpj? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Cnpj? left, Cnpj? right) => !(left == right);
    }
}
