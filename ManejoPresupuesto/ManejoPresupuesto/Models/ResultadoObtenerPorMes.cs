namespace ManejoPresupuesto.Models
{
    public class ResultadoObtenerPorMes
    {

        public int Mes { get; set; }
        public DateTime FechaReferencia { get; set; }
        public double Monto { get; set; }
        public double Ingreso { get; set; }
        public double Gasto { get; set; }
        public TipoOperacion TipoOperacionId { get; set; }

    }
}
