using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
	public class TransaccionesController : Controller
	{
		private readonly IServicioUsuarios servicioUsuarios;
		private readonly IRepositorioTransacciones repositorioTransacciones;
		private readonly IRepositorioCuentas repositorioCuentas;

		public TransaccionesController(IServicioUsuarios servicioUsuarios, IRepositorioTransacciones repositorioTransacciones, IRepositorioCuentas repositorioCuentas)
		{
			this.servicioUsuarios = servicioUsuarios;
			this.repositorioTransacciones = repositorioTransacciones;
			this.repositorioCuentas = repositorioCuentas;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Crear()
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var modelo = new TransaccionCreacionViewModel();
			modelo.Cuentas = await ObtenerCuentas(usuarioId);

			return View(modelo);
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
		{
			var cuentas = await repositorioCuentas.BuscarPorUsuario(usuarioId);
			return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}
	}
}
