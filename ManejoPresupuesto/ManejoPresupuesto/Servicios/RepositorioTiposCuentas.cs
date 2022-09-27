using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{


    public interface IRepositorioTiposCuentas
    {
		Task Actualizar(TipoCuenta tipoCuenta);
		Task Borrar(int id);
		Task Crear(TipoCuenta tipoCuenta);
        Task<bool> ExisteCuenta(string nombre, int usuarioId, int id = 0);
        Task<IEnumerable<TipoCuenta>> ObtenerCuentas(int usuarioId);
		Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }


    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string conecctionString;
        //private string insertTipoCuenta = $@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) Values (@Nombre, @UsuarioId, 0); SELECT SCOPE_IDENTITY();";
        private string insertTipoCuenta = "tiposCuentas_Insertar";
        private string validarExisteCuenta = $@"SELECT 1 FROM TiposCuentas WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId AND Id <> @id;";
        private string obtenerCuentas = $@"SELECT Id,Nombre, Orden FROM TiposCuentas WHERE UsuarioId = @UsuarioId ORDER BY Orden;";
        private string actualizarCuentas = $@"UPDATE TiposCuentas SET Nombre = @Nombre Where  Id = @Id;";
        private string obtenerPorId = $@"SELECT Id, Nombre, Orden FROM TiposCuentas WHERE Id = @Id AND UsuarioId = @UsuarioId;";
        private string borrarCuenta = $@"DELETE FROM TiposCuentas WHERE Id = @Id;";
        private string ordenarTiposCuentas = $@"UPDATE TiposCuentas SET  Orden = @Orden WHERE Id = @Id";
        public RepositorioTiposCuentas(IConfiguration configuration)
        {

            conecctionString = configuration.GetConnectionString("DefaultConnection");

        }

        //public async Task Crear(TipoCuenta tipoCuenta)
        //{
        //    using var connection = new SqlConnection(conecctionString);
        //    var id = await connection.QuerySingleAsync<int>(insertTipoCuenta, tipoCuenta);
        //    tipoCuenta.Id = id; 

        //}
        
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(conecctionString);
            var id = await connection.QuerySingleAsync<int>(insertTipoCuenta, 
                                                            new {usuarioId = tipoCuenta.UsuarioId, 
                                                                nombre = tipoCuenta.Nombre },
                                                                commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id; 

        }

        public async Task<bool> ExisteCuenta(string nombre, int usuarioId, int id = 0)
        {
            using var connection = new SqlConnection(conecctionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(validarExisteCuenta, new {nombre, usuarioId, id});
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> ObtenerCuentas(int usuarioId)
        {
            using var connection = new SqlConnection(conecctionString);
            return await connection.QueryAsync<TipoCuenta>(obtenerCuentas, new {usuarioId});
        }

        public async Task Actualizar (TipoCuenta tipoCuenta)
		{
            using var connection = new SqlConnection(conecctionString);
            await connection.ExecuteAsync(actualizarCuentas, tipoCuenta);
		}

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
		{
            using var connection = new SqlConnection(conecctionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(obtenerPorId, new {id, usuarioId});
		}

        public async Task Borrar(int id)
		{
            using var connection = new SqlConnection(conecctionString);
            await connection.ExecuteAsync(borrarCuenta, new {id});
		}

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            using var connection = new SqlConnection(conecctionString);
            await connection.ExecuteAsync(ordenarTiposCuentas, tipoCuentasOrdenados);
        }

    }
}
