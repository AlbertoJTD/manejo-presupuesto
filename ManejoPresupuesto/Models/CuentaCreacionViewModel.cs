using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
	public class CuentaCreacionViewModel: Cuenta
	{
		// IEnumerable => Coleccion de elementos
		// SelectListItem => Permite crear select tag
		public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
