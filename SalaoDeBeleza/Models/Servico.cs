namespace SalaoDeBeleza.Models
{
    public class Servico
    {
        public int IdServico { get; set; }
        public string Nome { get; set; } = "";
        public decimal Preco { get; set; }
        public int DuracaoMin { get; set; }
        public bool Ativo { get; set; } = true;
    }
}