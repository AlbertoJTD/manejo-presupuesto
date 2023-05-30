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
		private readonly IRepositorioCategorias repositorioCategorias;

		public TransaccionesController(IServicioUsuarios servicioUsuarios, IRepositorioTransacciones repositorioTransacciones, IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias)
		{
			this.servicioUsuarios = servicioUsuarios;
			this.repositorioTransacciones = repositorioTransacciones;
			this.repositorioCuentas = repositorioCuentas;
			this.repositorioCategorias = repositorioCategorias;
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
			modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);

			return View(modelo);
		}

		[HttpPost]
		public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			if (!ModelState.IsValid)
			{
				modelo.Cuentas = await ObtenerCuentas(usuarioId);
				modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
				return View(modelo);
			}

			var cuenta = await repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);
			if (cuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);
			if (categoria is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			modelo.UsuarioId = usuarioId;

			if (modelo.TipoOperacionId == TipoOperacion.Gasto)
			{
				modelo.Monto *= -1;
			}

			await repositorioTransacciones.Crear(modelo);

			return RedirectToAction("Index");
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
		{
			var cuentas = await repositorioCuentas.BuscarPorUsuario(usuarioId);
			return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}

		[HttpPost]
		public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
			return Ok(categorias);
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
		{
			var categorias = await repositorioCategorias.ObtenerCategoriasTipoOperacion(usuarioId, tipoOperacion);
			return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}
	}
}
