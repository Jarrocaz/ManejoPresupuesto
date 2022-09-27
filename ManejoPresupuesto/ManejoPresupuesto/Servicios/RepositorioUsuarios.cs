using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }

    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        private readonly string crearUsuario = $@"INSERT INTO Usuarios(Email, EmailNormalizado, PasswordHash) VALUES (@Email, @EmailNormalizado, @PasswordHash); SELECT SCOPE_IDENTITY();";
        private readonly string buscarUsuarioPorEmail = $@"SELECT * FROM Usuarios WHERE EmailNormalizado = @emailNormalizado";
        private readonly string procedimientoAlmacenadoDatosPorDefecto = $@"CrearDatosUsuarioNuevo";
        public RepositorioUsuarios(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var usuarioId = await connection.QuerySingleAsync<int>(crearUsuario, usuario);
            await connection.ExecuteAsync(procedimientoAlmacenadoDatosPorDefecto, new { usuarioId }, commandType: System.Data.CommandType.StoredProcedure);
            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(buscarUsuarioPorEmail, new { emailNormalizado });
        }


    }
}
