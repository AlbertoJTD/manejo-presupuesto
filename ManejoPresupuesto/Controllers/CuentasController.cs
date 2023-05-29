﻿using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
	public class CuentasController : Controller
	{
		private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
		private readonly IServicioUsuarios servicioUsuarios;
		private readonly IRepositorioCuentas repositorioCuentas;

		public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas)
        {
			this.repositorioTiposCuentas = repositorioTiposCuentas;
			this.servicioUsuarios = servicioUsuarios;
			this.repositorioCuentas = repositorioCuentas;
		}

		public async Task<IActionResult> Index()
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var cuentasUsuario = await repositorioCuentas.BuscarPorUsuario(usuarioId);

			var modelo = cuentasUsuario.GroupBy(x => x.TipoCuenta).Select(grupo => new IndiceCuentasViewModel
			{
				TipoCuenta = grupo.Key, // Igual al tipo de cuenta
				Cuentas = grupo.AsEnumerable()
			}).ToList();

			return View(modelo);
		}

		public async Task<IActionResult> Crear()
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var modelo = new CuentaCreacionViewModel();
			modelo.TiposCuentas = await ObtenerListadoCuentas(usuarioId);

			return View(modelo);
		}

		[HttpPost]
		public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
		{
			var usuarioId = servicioUsuarios.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

			if (tipoCuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			if (!ModelState.IsValid)
			{
				cuenta.TiposCuentas = await ObtenerListadoCuentas(usuarioId);
				return View(cuenta);
			}

			await repositorioCuentas.Crear(cuenta);
			return RedirectToAction("Index");
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerListadoCuentas(int usuarioId)
		{
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}
	}
}
