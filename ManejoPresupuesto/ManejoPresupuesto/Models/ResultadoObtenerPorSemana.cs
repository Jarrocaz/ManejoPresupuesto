namespace ManejoPresupuesto.Models
{
    public class ResultadoObtenerPorSemana
    {

        public  int Semana { get; set; }
        public  double Monto { get; set; }
        public  TipoOperacion TipoOperacionId { get; set; }
        public double Ingresos { get; set; }
        public double Gastos { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime Fechafin { get; set; }

    }
}
