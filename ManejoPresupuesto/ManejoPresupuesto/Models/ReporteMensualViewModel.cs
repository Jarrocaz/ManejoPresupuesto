namespace ManejoPresupuesto.Models
{
    public class ReporteMensualViewModel
    {

        public IEnumerable<ResultadoObtenerPorMes> TransaccionesPorMes { get; set; }
        public double Ingresos => TransaccionesPorMes.Sum(x => x.Ingreso);
        public double Gastos => TransaccionesPorMes.Sum(x => x.Gasto);
        public double Total => Ingresos - Gastos;
        public int Año { get; set; }

    }
}
