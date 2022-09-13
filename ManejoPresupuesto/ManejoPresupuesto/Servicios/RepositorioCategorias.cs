using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCategorias
    {
        Task ActualizarCategoria(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacionId);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;
        private readonly string insertarCategoria = $@"INSERT INTO  Categorias(Nombre, TipoOperacionId, UsuarioId) VALUES (@Nombre,@TipoOperacionId, @UsuarioId); SELECT SCOPE_IDENTITY();";
        private readonly string obtenerCategorias = $@"SELECT * FROM Categorias WHERE UsuarioId = @usuarioId";
        private readonly string obtenerCategoriasTransaccion = $@"SELECT * FROM Categorias WHERE UsuarioId = @usuarioId AND TipoOperacionId = @tipoOperacionId";
        private readonly string obtenerCategoriaPorId = $@"SELECT * FROM Categorias WHERE Id = @Id AND UsuarioId = @UsuarioId";
        private readonly string actualizarCategoria = $@"UPDATE Categorias SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId WHERE Id = @Id";
        private readonly string borrarCategoria = $@"DELETE FROM Categorias WHERE Id = @Id";
        public RepositorioCategorias(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(insertarCategoria,categoria);
            categoria.Id = id;

        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(obtenerCategorias, new {usuarioId});
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(obtenerCategoriasTransaccion, new { usuarioId, tipoOperacionId });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(obtenerCategoriaPorId, new {id, usuarioId});
        }

        public async Task ActualizarCategoria(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(actualizarCategoria, categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(borrarCategoria, new {id});

        }



    }
}
