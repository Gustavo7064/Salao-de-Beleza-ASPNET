using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;
using SalaoDeBeleza.Models;

namespace SalaoDeBeleza.Controllers
{
    [SessionAuthorize]
    public class ProfissionalController : Controller
    {
        private readonly Database db = new Database();
        private readonly IWebHostEnvironment env;

        public ProfissionalController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        // ============================================================
        // LISTAGEM
        // ============================================================

        public IActionResult Index()
        {
            var lista = new List<Profissional>();

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT * FROM profissional WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Profissional
                    {
                        IdProfissional = reader.GetInt32("id_profissional"),
                        Nome = reader.GetString("nome"),
                        Especialidade = reader["especialidade"] as string,
                        Cpf = reader["cpf"] as string,
                        Foto = reader["foto"] as string,
                        Ativo = reader.GetBoolean("ativo")
                    });
                }
            }

            return View(lista);
        }

        // ============================================================
        // CADASTRAR
        // ============================================================

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        {
            return View(new Profissional());
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Profissional profissional, IFormFile? fotoArquivo)
        {
            // Upload de foto
            string? nomeArquivo = await SalvarFoto(fotoArquivo);

            try
            {
                using (var conn = db.GetConnection())
                using (var cmd = new MySqlCommand(@"
                    INSERT INTO profissional (nome, especialidade, cpf, foto, ativo)
                    VALUES (@nome, @esp, @cpf, @foto, 1);", conn))
                {
                    cmd.Parameters.AddWithValue("@nome", profissional.Nome);
                    cmd.Parameters.AddWithValue("@esp", profissional.Especialidade ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@cpf", profissional.Cpf ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@foto", nomeArquivo ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ViewBag.Erro = "CPF já cadastrado. Verifique os dados e tente novamente.";
                return View(profissional);
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // EDITAR
        // ============================================================

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int id)
        {
            Profissional? profissional = null;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT * FROM profissional WHERE id_profissional = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    profissional = new Profissional
                    {
                        IdProfissional = reader.GetInt32("id_profissional"),
                        Nome = reader.GetString("nome"),
                        Especialidade = reader["especialidade"] as string,
                        Cpf = reader["cpf"] as string,
                        Foto = reader["foto"] as string,
                        Ativo = reader.GetBoolean("ativo")
                    };
                }
            }

            if (profissional == null) return NotFound();
            return View(profissional);
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Profissional profissional, IFormFile? fotoArquivo)
        {
            // Nova foto enviada? Substitui a anterior
            string? novaFoto = await SalvarFoto(fotoArquivo);
            string? fotoFinal = novaFoto ?? profissional.Foto;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                UPDATE profissional
                SET nome = @nome, especialidade = @esp, cpf = @cpf, foto = @foto
                WHERE id_profissional = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@nome", profissional.Nome);
                cmd.Parameters.AddWithValue("@esp", profissional.Especialidade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cpf", profissional.Cpf ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@foto", fotoFinal ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id", profissional.IdProfissional);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // EXCLUIR (exclusão lógica — não apaga do banco)
        // ============================================================

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "UPDATE profissional SET ativo = 0 WHERE id_profissional = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // MÉTODO AUXILIAR: salvar foto em wwwroot/img/fotos
        // ============================================================

        private async Task<string?> SalvarFoto(IFormFile? arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return null;

            string extensao = Path.GetExtension(arquivo.FileName).ToLower();
            string nomeUnico = $"{Guid.NewGuid()}{extensao}";
            string pasta = Path.Combine(env.WebRootPath, "img", "fotos");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string caminho = Path.Combine(pasta, nomeUnico);

            using var stream = new FileStream(caminho, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            return nomeUnico;
        }
    }
}