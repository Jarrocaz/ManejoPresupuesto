namespace ManejoPresupuesto.Models
{
    public class ReporteSemanalViewModel
    {
        public double Ingresos  => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public double Gastos => TransaccionesPorSemana.Sum(x => x.Gastos);
        public double Total => Ingresos - Gastos;
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ResultadoObtenerPorSemana> TransaccionesPorSemana { get; set; }
    }
}
