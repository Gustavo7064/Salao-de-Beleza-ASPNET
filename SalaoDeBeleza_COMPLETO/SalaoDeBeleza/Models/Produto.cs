namespace SalaoDeBeleza.Models
{
    public class Produto
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; } = "";
        public string? Marca { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public bool Ativo { get; set; } = true;
    }
}