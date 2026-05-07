using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;
using SalaoDeBeleza.Models;

namespace SalaoDeBeleza.Controllers
{
    public class AdminController : Controller
    {
        private readonly Database db = new Database();

        // ============================================================
        // LOGIN
        // ============================================================

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, string? returnUrl = null)
        {
            int userId = 0;
            string? hash = null;
            string? role = null;
            bool ativo = false;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                SELECT id, password_hash, role, ativo
                FROM usuarios
                WHERE username = @u
                LIMIT 1;", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userId = reader.GetInt32("id");
                    hash = reader["password_hash"] as string;
                    role = reader["role"] as string;
                    ativo = reader.GetBoolean("ativo");
                }
            }

            // Valida credenciais
            if (userId == 0 || !ativo || string.IsNullOrEmpty(hash)
                || !BCrypt.Net.BCrypt.Verify(password, hash))
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View();
            }

            // Grava sessão
            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("Role", role ?? "Recepcionista");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // LOGOUT
        // ============================================================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ============================================================
        // ACESSO NEGADO
        // ============================================================

        public IActionResult AcessoNegado()
        {
            return View();
        }

        // ============================================================
        // NOVO USUÁRIO (somente Admin)
        // ============================================================

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult NovoUsuario()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult NovoUsuario(Usuario vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Username) || string.IsNullOrWhiteSpace(vm.Password))
            {
                ViewBag.Erro = "Preencha usuário e senha.";
                return View(vm);
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(vm.Password);

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                INSERT INTO usuarios (username, password_hash, role, ativo)
                VALUES (@u, @h, @r, 1);", conn))
            {
                cmd.Parameters.AddWithValue("@u", vm.Username);
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@r", vm.Role);
                cmd.ExecuteNonQuery();
            }

            ViewBag.Sucesso = "Usuário cadastrado com sucesso!";
            return View(new Usuario());
        }

        // ============================================================
        // LISTAR USUÁRIOS (somente Admin)
        // ============================================================

        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult Usuarios()
        {
            var lista = new List<Usuario>();

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT id, username, role, ativo FROM usuarios ORDER BY username;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Usuario
                    {
                        Id = reader.GetInt32("id"),
                        Username = reader.GetString("username"),
                        Role = reader.GetString("role"),
                        Ativo = reader.GetBoolean("ativo")
                    });
                }
            }

            return View(lista);
        }

        // ============================================================
        // DESATIVAR USUÁRIO (exclusão lógica)
        // ============================================================

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DesativarUsuario(int id)
        {
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "UPDATE usuarios SET ativo = 0 WHERE id = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Usuarios");
        }
    }
}