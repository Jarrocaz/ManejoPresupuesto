using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        private string crearCuenta = $@"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance) VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance); SELECT SCOPE_IDENTITY();";
        private string buscarCuenta = $@"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, tc.Nombre AS TipoCuenta  FROM Cuentas INNER JOIN TiposCuentas tc ON tc.Id = Cuentas.TipoCuentaId WHERE tc.UsuarioId = @UsuarioId ORDER BY tc.Orden";
        private string obtenerPorId = $@"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, Cuentas.Descripcion, TipoCuentaId  FROM Cuentas INNER JOIN TiposCuentas tc ON tc.Id = Cuentas.TipoCuentaId WHERE tc.UsuarioId = @UsuarioId AND Cuentas.Id = @Id ";
        private string actualizarCuenta = $@"UPDATE Cuentas SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId WHERE Id = @Id;";
        private string borrarCuenta = $@"DELETE Cuentas WHERE Id = @Id";
        public RepositorioCuentas(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(crearCuenta, cuenta);
            cuenta.Id = id;
        }
        
        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(buscarCuenta, new { usuarioId });    
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(obtenerPorId, new {id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //ExecuteAsync sirve para no retornar nada
            await connection.ExecuteAsync(actualizarCuenta, cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(borrarCuenta, new { id });
        }
    
    }
}
