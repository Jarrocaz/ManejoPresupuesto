using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, double montoAnterior, int cuentaAnterior);
        Task Crear(Transaccion transaccion);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    }



    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        private string procedimientoInsertarTransaccion = "Transacciones_Insertar";
        private string procedimientoActualizarTransaccion = "Transacciones_Actualizar";
        private string obtenerPorId = $@"SELECT Transacciones.*, cat.TipoOperacionId FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId";
        public RepositorioTransacciones(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(procedimientoInsertarTransaccion, new
            {
                transaccion.UsuarioId,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota
            },
            commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;

        }

        public async Task Actualizar(Transaccion transaccion, double montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(procedimientoActualizarTransaccion, new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            }, commandType: System.Data.CommandType.StoredProcedure);

        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(obtenerPorId, new {id, usuarioId});
        }

    }
}
