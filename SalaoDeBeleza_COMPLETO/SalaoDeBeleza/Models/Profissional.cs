namespace SalaoDeBeleza.Models
{
    public class Profissional
    {
        public int IdProfissional { get; set; }
        public string Nome { get; set; } = "";
        public string? Especialidade { get; set; }
        public string? Cpf { get; set; }
        public string? Foto { get; set; }
        public bool Ativo { get; set; } = true;
    }
}