using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;
using SalaoDeBeleza.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SalaoDeBeleza.Controllers
{
    [SessionAuthorize]
    public class ProdutoController : Controller
    {
        private readonly Database db = new Database();
        private readonly string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "fotos");

        public ProdutoController()
        {
            // Criar diretório se não existir
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
        }

        // ============================================================
        // INDEX - LISTAR PRODUTOS
        // ============================================================
        public IActionResult Index()
        {
            var lista = new List<Produto>();

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT id_produto, nome, marca, preco, estoque, ativo FROM produto WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Produto
                    {
                        IdProduto = reader.GetInt32("id_produto"),
                        Nome = reader.GetString("nome"),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString("marca"),
                        Preco = reader.GetDecimal("preco"),
                        Estoque = reader.GetInt32("estoque"),
                        Ativo = reader.GetBoolean("ativo")
                    });
                }
            }

            return View(lista);
        }

        // ============================================================
        // CRIAR - GET
        // ============================================================
        [HttpGet]
        public IActionResult Criar()
        {
            return View(new Produto());
        }

        // ============================================================
        // CRIAR - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                ViewBag.Erro = "Nome do produto é obrigatório.";
                return View(produto);
            }

            try
            {
                using (var conn = db.GetConnection())
                using (var cmd = new MySqlCommand(@"
                    INSERT INTO produto (nome, marca, preco, estoque, ativo)
                    VALUES (@nome, @marca, @preco, @estoque, 1);", conn))
                {
                    cmd.Parameters.AddWithValue("@nome", produto.Nome);
                    cmd.Parameters.AddWithValue("@marca", produto.Marca ?? "");
                    cmd.Parameters.AddWithValue("@preco", produto.Preco);
                    cmd.Parameters.AddWithValue("@estoque", produto.Estoque);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Sucesso = "Produto cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao cadastrar: " + ex.Message;
                return View(produto);
            }
        }

        // ============================================================
        // EDITAR - GET
        // ============================================================
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Produto produto = null;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT id_produto, nome, marca, preco, estoque, ativo FROM produto WHERE id_produto = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    produto = new Produto
                    {
                        IdProduto = reader.GetInt32("id_produto"),
                        Nome = reader.GetString("nome"),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString("marca"),
                        Preco = reader.GetDecimal("preco"),
                        Estoque = reader.GetInt32("estoque"),
                        Ativo = reader.GetBoolean("ativo")
                    };
                }
            }

            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // ============================================================
        // EDITAR - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                ViewBag.Erro = "Nome do produto é obrigatório.";
                return View(produto);
            }

            try
            {
                using (var conn = db.GetConnection())
                using (var cmd = new MySqlCommand(@"
                    UPDATE produto 
                    SET nome = @nome, marca = @marca, preco = @preco, estoque = @estoque
                    WHERE id_produto = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@nome", produto.Nome);
                    cmd.Parameters.AddWithValue("@marca", produto.Marca ?? "");
                    cmd.Parameters.AddWithValue("@preco", produto.Preco);
                    cmd.Parameters.AddWithValue("@estoque", produto.Estoque);
                    cmd.Parameters.AddWithValue("@id", produto.IdProduto);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Sucesso = "Produto atualizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao atualizar: " + ex.Message;
                return View(produto);
            }
        }

        // ============================================================
        // EXCLUIR - POST (Exclusão Lógica)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            try
            {
                using (var conn = db.GetConnection())
                using (var cmd = new MySqlCommand(
                    "UPDATE produto SET ativo = 0 WHERE id_produto = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Sucesso = "Produto desativado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao desativar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
