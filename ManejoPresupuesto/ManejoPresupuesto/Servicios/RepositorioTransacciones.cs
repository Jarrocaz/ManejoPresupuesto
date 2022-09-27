using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, double montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSermana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }



    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        private string procedimientoInsertarTransaccion = "Transacciones_Insertar";
        private string procedimientoActualizarTransaccion = "Transacciones_Actualizar";
        private string obtenerPorId = $@"SELECT Transacciones.*, cat.TipoOperacionId FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId";
        private string procedimientoBorrarTransaccion = "Transacciones_Borrar";
        private string obtenerPorCuentaId = $@"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId 
                                            FROM Transacciones t INNER JOIN Categorias c ON c.Id = t.CategoriaId 
                                            INNER JOIN Cuentas cu ON cu.Id = t.CuentaId 
                                            WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId AND FechaTransaccion 
                                            BETWEEN @FechaInicio AND @FechaFin";

        private string obtenerPorUsuarioId = $@"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId, Nota 
                                            FROM Transacciones t INNER JOIN Categorias c ON c.Id = t.CategoriaId 
                                            INNER JOIN Cuentas cu ON cu.Id = t.CuentaId 
                                            WHERE t.UsuarioId = @UsuarioId AND FechaTransaccion 
                                            BETWEEN @FechaInicio AND @FechaFin ORDER BY t.FechaTransaccion DESC";

        private string obtenerPorSemana = $@"SELECT DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana, SUM(Monto) AS Monto, cat.TipoOperacionId 
                                           FROM Transacciones inner join Categorias cat on cat.id = Transacciones.CategoriaId 
                                           WHERE Transacciones.UsuarioId = @usuarioId AND FechaTransaccion 
                                           BETWEEN @fechaInicio and @fechaFin GROUP BY DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7,
                                           cat.TipoOperacionId";

        private string obtenerPorMes = $@"SELECT MONTH(FechaTransaccion) as Mes, SUM(Monto) as Monto, cat.TipoOperacionId 
                                          FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId 
                                          WHERE Transacciones.UsuarioId = @usuarioId AND  YEAR(FechaTransaccion) = @Año 
                                          GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionId";


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


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(procedimientoBorrarTransaccion, new {id}, commandType: System.Data.CommandType.StoredProcedure);
        }


        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(obtenerPorCuentaId, modelo);
        }


        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(obtenerPorUsuarioId, modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSermana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(obtenerPorSemana, modelo);

        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(obtenerPorMes, new {usuarioId, año});

        }


    }
}
