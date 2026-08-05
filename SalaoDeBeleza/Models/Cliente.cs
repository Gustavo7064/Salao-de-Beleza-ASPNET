namespace SalaoDeBeleza.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nome { get; set; } = "";
        public string? Telefone { get; set; }
        public string? DataNascimento { get; set; }
        public bool Ativo { get; set; } = true;
    }
}