namespace SalaoDeBeleza.Models
{
    public class Agendamento
    {
        public int IdAgendamento { get; set; }
        public string DataHora { get; set; } = "";
        public string Status { get; set; } = "agendado";
        public int IdCliente { get; set; }
        public int IdProfissional { get; set; }
        public int IdServico { get; set; }
        public bool Ativo { get; set; } = true;

        // Campos extras para exibição nas listagens (JOIN)
        public string NomeCliente { get; set; } = "";
        public string NomeProfissional { get; set; } = "";
        public string NomeServico { get; set; } = "";
    }
}