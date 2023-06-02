namespace ManejoPresupuesto.Models
{
	public class ReporteSemanalViewModel
	{
        public decimal Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public decimal Gatos => TransaccionesPorSemana.Sum(x => x.Gastos);
        public decimal Total => Ingresos - Gatos;
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ResultadoObtenerPorSemana> TransaccionesPorSemana { get; set; }
    }
}
