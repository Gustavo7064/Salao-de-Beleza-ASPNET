using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;
using SalaoDeBeleza.Models;

namespace SalaoDeBeleza.Controllers
{
    [SessionAuthorize]
    public class ServicoController : Controller
    {
        private readonly Database db = new Database();

        // ============================================================
        // LISTAGEM
        // ============================================================

        public IActionResult Index()
        {
            var lista = new List<Servico>();

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT * FROM servico WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Servico
                    {
                        IdServico = reader.GetInt32("id_servico"),
                        Nome = reader.GetString("nome"),
                        Preco = reader.GetDecimal("preco"),
                        DuracaoMin = reader.GetInt32("duracao_min"),
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
            return View(new Servico());
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(Servico servico)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                INSERT INTO servico (nome, preco, duracao_min, ativo)
                VALUES (@nome, @preco, @duracao, 1);", conn))
            {
                cmd.Parameters.AddWithValue("@nome", servico.Nome);
                cmd.Parameters.AddWithValue("@preco", servico.Preco);
                cmd.Parameters.AddWithValue("@duracao", servico.DuracaoMin);
                cmd.ExecuteNonQuery();
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
            Servico? servico = null;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT * FROM servico WHERE id_servico = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    servico = new Servico
                    {
                        IdServico = reader.GetInt32("id_servico"),
                        Nome = reader.GetString("nome"),
                        Preco = reader.GetDecimal("preco"),
                        DuracaoMin = reader.GetInt32("duracao_min"),
                        Ativo = reader.GetBoolean("ativo")
                    };
                }
            }

            if (servico == null) return NotFound();
            return View(servico);
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Servico servico)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                UPDATE servico
                SET nome = @nome, preco = @preco, duracao_min = @duracao
                WHERE id_servico = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@nome", servico.Nome);
                cmd.Parameters.AddWithValue("@preco", servico.Preco);
                cmd.Parameters.AddWithValue("@duracao", servico.DuracaoMin);
                cmd.Parameters.AddWithValue("@id", servico.IdServico);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // EXCLUIR (exclusão lógica)
        // ============================================================

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "UPDATE servico SET ativo = 0 WHERE id_servico = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}