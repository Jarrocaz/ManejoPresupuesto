using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
		private readonly IServicioUsuarios servicioUsuarios;

		public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
			this.servicioUsuarios = servicioUsuarios;
		}

        public IActionResult Crear()
        {
            return View();
        }


        public async Task<IActionResult> MostrarCuentas()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.ObtenerCuentas(usuarioId);
            return View(tiposCuentas);
        }


        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            var yaExistetipocuenta = await repositorioTiposCuentas.ExisteCuenta(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (yaExistetipocuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe");
                return View(tipoCuenta);
            }

            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("MostrarCuentas");
        }


		[HttpGet]
        public async Task<IActionResult> Editar (int id)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
			if (tipoCuenta is null)
			{
                return RedirectToAction("NoEncontrado", "Home");
			}

            return View(tipoCuenta);
		}


        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("MostrarCuentas");

        }

        public async Task<IActionResult> Borrar (int id)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

			if (tipoCuenta is null)
			{
                return RedirectToAction("NoEncontrado", "Home");
			}

            return View(tipoCuenta);

		}

		[HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Borrar(id);

            return RedirectToAction("MostrarCuentas");

        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre, int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.ExisteCuenta(nombre, usuarioId, id);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);

        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.ObtenerCuentas(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecesAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecesAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentaOrdenadas = ids.Select((valor, indice) => new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentaOrdenadas);


            return Ok();
        }


    }
}
