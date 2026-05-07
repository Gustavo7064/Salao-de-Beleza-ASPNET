using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;
using SalaoDeBeleza.Models;
using System.Collections.Generic;
using System.Linq;

namespace SalaoDeBeleza.Controllers
{
    [SessionAuthorize]
    public class AgendamentoController : Controller
    {
        private readonly Database db = new Database();

        // ============================================================
        // INDEX - LISTAR AGENDAMENTOS
        // ============================================================
        public IActionResult Index()
        {
            var lista = new List<Agendamento>();

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                SELECT 
                    a.id_agendamento,
                    a.data_hora,
                    a.status,
                    a.ativo,
                    c.nome as nome_cliente,
                    p.nome as nome_profissional,
                    s.nome as nome_servico,
                    a.id_cliente,
                    a.id_profissional,
                    a.id_servico
                FROM agendamento a
                INNER JOIN cliente c ON a.id_cliente = c.id_cliente
                INNER JOIN profissional p ON a.id_profissional = p.id_profissional
                INNER JOIN servico s ON a.id_servico = s.id_servico
                WHERE a.ativo = 1
                ORDER BY a.data_hora DESC;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Agendamento
                    {
                        IdAgendamento = reader.GetInt32("id_agendamento"),
                        DataHora = reader.GetDateTime("data_hora").ToString("dd/MM/yyyy HH:mm"),
                        Status = reader.GetString("status"),
                        NomeCliente = reader.GetString("nome_cliente"),
                        NomeProfissional = reader.GetString("nome_profissional"),
                        NomeServico = reader.GetString("nome_servico"),
                        IdCliente = reader.GetInt32("id_cliente"),
                        IdProfissional = reader.GetInt32("id_profissional"),
                        IdServico = reader.GetInt32("id_servico"),
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
            CarregarDados();
            return View(new Agendamento());
        }

        // ============================================================
        // CRIAR - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(Agendamento agendamento)
        {
            if (agendamento.IdCliente == 0 || agendamento.IdProfissional == 0 || agendamento.IdServico == 0)
            {
                ViewBag.Erro = "Selecione cliente, profissional e serviço.";
                CarregarDados();
                return View(agendamento);
            }

            try
            {
                // Converter string para DateTime
                if (!DateTime.TryParse(agendamento.DataHora, out DateTime dataHora))
                {
                    ViewBag.Erro = "Data e hora inválidas.";
                    CarregarDados();
                    return View(agendamento);
                }

                using (var conn = db.GetConnection())
                using (var cmd = new MySqlCommand(@"
                    INSERT INTO agendamento (data_hora, status, id_cliente, id_profissional, id_servico, ativo)
                    VALUES (@data_hora, 'agendado', @id_cliente, @id_profissional, @id_servico, 1);", conn))
                {
                    cmd.Parameters.AddWithValue("@data_hora", dataHora);
                    cmd.Parameters.AddWithValue("@id_cliente", agendamento.IdCliente);
                    cmd.Parameters.AddWithValue("@id_profissional", agendamento.IdProfissional);
                    cmd.Parameters.AddWithValue("@id_servico", agendamento.IdServico);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Sucesso = "Agendamento criado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao agendar: " + ex.Message;
                CarregarDados();
                return View(agendamento);
            }
        }

        // ============================================================
        // EDITAR - GET
        // ============================================================
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Agendamento agendamento = null;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                SELECT 
                    a.id_agendamento,
                    a.data_hora,
                    a.status,
                    a.ativo,
                    c.nome as nome_cliente,
                    p.nome as nome_profissional,
                    s.nome as nome_servico,
                    a.id_cliente,
                    a.id_profissional,
                    a.id_servico
                FROM agendamento a
                INNER JOIN cliente c ON a.id_cliente = c.id_cliente
                INNER JOIN profissional p ON a.id_profissional = p.id_profissional
                INNER JOIN servico s ON a.id_servico = s.id_servico
                WHERE a.id_agendamento = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    agendamento = new Agendamento
                    {
                        IdAgendamento = reader.GetInt32("id_agendamento"),
                        DataHora = reader.GetDateTime("data_hora").ToString("yyyy-MM-ddTHH:mm"),
                        Status = reader.GetString("status"),
                        NomeCliente = reader.GetString("nome_cliente"),
                        NomeProfissional = reader.GetString("nome_profissional"),
                        NomeServico = reader.GetString("nome_servico"),
                        IdCliente = reader.GetInt32("id_cliente"),
                        IdProfissional = reader.GetInt32("id_profissional"),
                        IdServico = reader.GetInt32("id_servico"),
                        Ativo = reader.GetBoolean("ativo")
                    };
                }
            }

            if (agendamento == null)
                return NotFound();

            CarregarDados();
            return View(agendamento);
        }

        // ============================================================
        // EDITAR - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Agendamento agendamento)
        {
            if (agendamento.IdCliente == 0 || agendamento.IdProfissional == 0 || agendamento.IdServico == 0)
            {
                ViewBag.Erro = "Selecione cliente, profissional e serviço.";
                CarregarDados();
                return View(agendamento);
            }

            try
            {
                // Converter string para DateTime
                if (!DateTime.TryParse(agendamento.DataHora, out DateTime dataHora))
                {
                    ViewBag.Erro = "Data e hora inválidas.";
                    CarregarDados();
                    return View(agendamento);
                }

                using (var conn = db.GetConnection())
                using (var cmd = new MySqlCommand(@"
                    UPDATE agendamento 
                    SET data_hora = @data_hora, status = @status, id_cliente = @id_cliente, 
                        id_profissional = @id_profissional, id_servico = @id_servico
                    WHERE id_agendamento = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@data_hora", dataHora);
                    cmd.Parameters.AddWithValue("@status", agendamento.Status ?? "agendado");
                    cmd.Parameters.AddWithValue("@id_cliente", agendamento.IdCliente);
                    cmd.Parameters.AddWithValue("@id_profissional", agendamento.IdProfissional);
                    cmd.Parameters.AddWithValue("@id_servico", agendamento.IdServico);
                    cmd.Parameters.AddWithValue("@id", agendamento.IdAgendamento);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Sucesso = "Agendamento atualizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao atualizar: " + ex.Message;
                CarregarDados();
                return View(agendamento);
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
                    "UPDATE agendamento SET ativo = 0 WHERE id_agendamento = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Sucesso = "Agendamento cancelado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Erro ao cancelar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ============================================================
        // MÉTODO AUXILIAR - CARREGAR DADOS PARA DROPDOWNS
        // ============================================================
        private void CarregarDados()
        {
            // Carregar Clientes
            var clientes = new List<Cliente>();
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT id_cliente, nome FROM cliente WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    clientes.Add(new Cliente
                    {
                        IdCliente = reader.GetInt32("id_cliente"),
                        Nome = reader.GetString("nome")
                    });
                }
            }
            ViewBag.Clientes = clientes;

            // Carregar Profissionais
            var profissionais = new List<Profissional>();
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT id_profissional, nome FROM profissional WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    profissionais.Add(new Profissional
                    {
                        IdProfissional = reader.GetInt32("id_profissional"),
                        Nome = reader.GetString("nome")
                    });
                }
            }
            ViewBag.Profissionais = profissionais;

            // Carregar Serviços
            var servicos = new List<Servico>();
            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(
                "SELECT id_servico, nome, preco FROM servico WHERE ativo = 1 ORDER BY nome;", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    servicos.Add(new Servico
                    {
                        IdServico = reader.GetInt32("id_servico"),
                        Nome = reader.GetString("nome"),
                        Preco = reader.GetDecimal("preco")
                    });
                }
            }
            ViewBag.Servicos = servicos;
        }
    }
}
