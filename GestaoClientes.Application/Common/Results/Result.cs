namespace GestaoClientes.Application.Common.Results
{
    public class Result
    {
        public bool Sucesso { get; }
        public string Mensagem { get; }
        public List<string> Erros { get; }

        protected Result(bool sucesso, string mensagem, List<string>? erros = null)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Erros = erros ?? new List<string>();
        }

        public static Result Ok(string mensagem = "Operação realizada com sucesso.") => new Result(true, mensagem);

        public static Result Falha(string mensagem) => new Result(false, mensagem, new List<string> { mensagem });

        public static Result Falha(List<string> erros) => new Result(false, "Ocorreram erros na operação.", erros);
    }

    public class Result<T> : Result
    {
        public T? Dados { get; }

        protected Result(bool sucesso, string mensagem, T? dados, List<string>? erros = null) : base(sucesso, mensagem, erros)
        {
            Dados = dados;
        }

        public static Result<T> Ok(T dados, string mensagem = "Operação realizada com sucesso.") => new Result<T>(true, mensagem, dados);

        public static new Result<T> Falha(string mensagem) => new Result<T>(false, mensagem, default, new List<string> { mensagem });

        public static new Result<T> Falha(List<string> erros) => new Result<T>(false, "Ocorreram erros na operação.", default, erros);
    }
}
