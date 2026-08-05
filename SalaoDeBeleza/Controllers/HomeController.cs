using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SalaoDeBeleza.Data;
using SalaoDeBeleza.Filters;

namespace SalaoDeBeleza.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        private readonly Database db = new Database();

        public IActionResult Index()
        {
            // Totais para o painel inicial
            ViewBag.TotalProfissionais = ContarAtivos("profissional");
            ViewBag.TotalClientes = ContarAtivos("cliente");
            ViewBag.TotalServicos = ContarAtivos("servico");
            ViewBag.TotalAgendamentos = ContarAtivos("agendamento");

            return View();
        }

        private int ContarAtivos(string tabela)
        {
            using var conn = db.GetConnection();
            using var cmd = new MySqlCommand(
                $"SELECT COUNT(*) FROM {tabela} WHERE ativo = 1;", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new SalaoDeBeleza.Models.ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id
                            ?? HttpContext.TraceIdentifier
            });
        }
    }
}