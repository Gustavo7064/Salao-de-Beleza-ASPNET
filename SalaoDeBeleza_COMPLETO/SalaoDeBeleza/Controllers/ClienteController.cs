using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;
using SalaoDeBeleza.Models;

namespace SalaoDeBeleza.Controllers
{
    [SessionAuthorize]
    public class ClienteController : Controller
    {
        private readonly Database db = new Database();

        // ============================================================
        // LISTAGEM
        // ============================================================

        public IActionResult Index()
        {
            var lista = new List<Cliente>();

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT * FROM cliente WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Cliente
                    {
                        IdCliente = reader.GetInt32("id_cliente"),
                        Nome = reader.GetString("nome"),
                        Telefone = reader["telefone"] as string,
                        DataNascimento = reader["data_nascimento"] == DBNull.Value
                                         ? null
                                         : Convert.ToDateTime(reader["data_nascimento"])
                                                  .ToString("yyyy-MM-dd"),
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
        public IActionResult Criar()
        {
            return View(new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(Cliente cliente)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                INSERT INTO cliente (nome, telefone, data_nascimento, ativo)
                VALUES (@nome, @tel, @dn, 1);", conn))
            {
                cmd.Parameters.AddWithValue("@nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@tel", cliente.Telefone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@dn",
                    string.IsNullOrEmpty(cliente.DataNascimento)
                        ? (object)DBNull.Value
                        : cliente.DataNascimento);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // EDITAR
        // ============================================================

        [HttpGet]
        public IActionResult Editar(int id)
        {
            Cliente? cliente = null;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT * FROM cliente WHERE id_cliente = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    cliente = new Cliente
                    {
                        IdCliente = reader.GetInt32("id_cliente"),
                        Nome = reader.GetString("nome"),
                        Telefone = reader["telefone"] as string,
                        DataNascimento = reader["data_nascimento"] == DBNull.Value
                                         ? null
                                         : Convert.ToDateTime(reader["data_nascimento"])
                                                  .ToString("yyyy-MM-dd"),
                        Ativo = reader.GetBoolean("ativo")
                    };
                }
            }

            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Cliente cliente)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                UPDATE cliente
                SET nome = @nome, telefone = @tel, data_nascimento = @dn
                WHERE id_cliente = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@tel", cliente.Telefone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@dn",
                    string.IsNullOrEmpty(cliente.DataNascimento)
                        ? (object)DBNull.Value
                        : cliente.DataNascimento);
                cmd.Parameters.AddWithValue("@id", cliente.IdCliente);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ============================================================
        // EXCLUIR (exclusão lógica)
        // ============================================================

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "UPDATE cliente SET ativo = 0 WHERE id_cliente = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}