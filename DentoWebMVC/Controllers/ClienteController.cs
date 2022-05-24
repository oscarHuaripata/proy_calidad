using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DentoWebMVC.Models.Context;
using DentoWebMVC.Models.DentoWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rotativa.AspNetCore;

namespace DentoWebMVC.Controllers
{
    public class ClienteController : Controller
    {

        public DentoWebContext cnx;
        public readonly IConfiguration configuration;
        public ClienteController(DentoWebContext cnx, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cnx = cnx;
        }

        public IActionResult Index()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = cnx.Clientes.Where(o => o.usuario == claim.Value).First();
            ViewBag.Claim = claim;
            ViewBag.User = user;

            var estado = cnx.Citas.Where(o => o.idCita == user.idCliente).FirstOrDefault();
            ViewBag.Estado = estado;

            var citascliente = cnx.Citas.Where(o => o.idCliente == user.idCliente).FirstOrDefault();

            if (citascliente != null)
            {
                ViewBag.Citas = cnx.Citas.Include(o => o.horario).Include(o => o.doctor).Where(o => o.idCliente == user.idCliente).ToList();
            }
            else
            {
                TempData["CitasNull"] = "Usted no tienes citas pendientes...";
            }


            return View();
        }
        [HttpGet]
        public ActionResult CreateCita (){
            ViewBag.Horarios = cnx.Horarios.ToList();
            ViewBag.Doctors = cnx.Doctors.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateCita(string idhorario, int iddoctor, DateTime fecha)
        {
            var horariodoctor = cnx.Citas.Include(o => o.horario).Where(o => o.idDoctor == Convert.ToInt32(iddoctor)).ToList();
            var horarios = cnx.Horarios.ToList();
            bool estado = false;
            foreach (var item in horariodoctor)
            {
                if (item.fecha == fecha)
                {
                    if (item.idHorario == Convert.ToInt32(idhorario))
                    {
                        estado = false;
                        TempData["CitaNoCreada"] = "Horario Ocupado";
                        break;
                    }
                    else
                    {
                        estado = true;
                    }
                }
                else
                {
                    if (DateTime.Now < fecha)
                    {
                        estado = true;
                    }
                    else
                    {
                        estado = false;
                        TempData["CitaNoCreada"] = "Fecha invalida";
                    }
                }
            }

            if (estado)
            {
                var claim = HttpContext.User.Claims.FirstOrDefault();
                var user = cnx.Clientes.Where(o => o.usuario == claim.Value).First();

                Cita cita = new Cita();
                Random ramdom = new Random();
                var a = ramdom.Next(50, 150);

                cita.fecha = fecha;

                cita.idHorario = Convert.ToInt32(idhorario);

                cita.estado = "PENDIENTE";

                cita.idCliente = user.idCliente;

                cita.idDoctor = Convert.ToInt32(iddoctor);

                cita.monto = a;

                cita.pago = "false";

                cnx.Citas.Add(cita);
                cnx.SaveChanges();
                TempData["CitaCreada"] = "Cita creada con exito";
            }


            return RedirectToAction("Index","Cliente");
        }


        public ActionResult CancelarCita(int id)
        {
            var cita = cnx.Citas.Where(o => o.idCita == id).FirstOrDefault();

            cita.estado = "CANCELADA";

            cnx.SaveChanges();


            return RedirectToAction("Index","Cliente");
        }

        [HttpGet]
        public ActionResult CreateCliente()
        {
            return View("CreateCliente");
        }


        [HttpPost]
        public ActionResult CreateCliente(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {

                return View();

            }

            cliente.codigo = "C - " + cliente.usuario;
            
            var pass = CreateHash(cliente.passwd);
            cliente.passwd = pass;
            cnx.Clientes.Add(cliente);
            cnx.SaveChanges();

            return RedirectToAction("Login", "Auth");


        }

        public ActionResult DetalleCitaPaciente(int id)
        {
            var historia = cnx.Historias.Include(o => o.cita).Where(o => o.idCita == id).FirstOrDefault();
            var paciente = cnx.Clientes.Where(o => o.idCliente == historia.cita.idCliente).FirstOrDefault();
            var doctor = cnx.Doctors.Where(o => o.idDoctor == historia.cita.idDoctor).FirstOrDefault();
            ViewBag.Doctor = doctor;
            ViewBag.Historia = historia;
            ViewBag.Paciente = paciente;
            return View();
        }

        private string CreateHash(string input)
        {
            var sha = SHA256.Create();
            input += configuration.GetValue<string>("Token");
            var hash = sha.ComputeHash(Encoding.Default.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        public ActionResult ClientePerfil()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = cnx.Clientes.Where(o => o.usuario == claim.Value).FirstOrDefault();
            ViewBag.User = user;
            ViewBag.Id = user.idCliente;
            return View();
        }

        [HttpGet]
        public ActionResult RealizarPago(int id)
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = cnx.Clientes.Where(o => o.usuario == claim.Value).FirstOrDefault();
            var citita = cnx.Citas.Where(a => a.idCita == id).FirstOrDefault();
            ViewBag.Paciente = user;
            ViewBag.Cita = citita;
            citita.pago = "true";
            cnx.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Boleta(int id)
        {
            var historia = cnx.Historias.Include(o => o.cita).Where(o => o.idCita == id).FirstOrDefault();
            var paciente = cnx.Clientes.Where(o => o.idCliente == historia.cita.idCliente).FirstOrDefault();
            var doctor = cnx.Doctors.Where(o => o.idDoctor == historia.cita.idDoctor).FirstOrDefault();
            var citita = cnx.Citas.Where(a => a.idCita == id).FirstOrDefault();
            ViewBag.Doctor = doctor;
            ViewBag.Historia = historia;
            ViewBag.Paciente = paciente;
            ViewBag.Cita = citita;

            return View("IndexBoleta");
        }

        [HttpGet]
        public ActionResult EditarPaciente(int id)
        {
            ViewBag.Id = id;
            var paciente = cnx.Clientes.Where(o => o.idCliente == id).FirstOrDefault();

            ViewBag.Nombres = paciente.nombres;
            ViewBag.Apellidos = paciente.apellidos;
            ViewBag.Dni = paciente.dni;
            ViewBag.FechaN = paciente.fechaNac;
            ViewBag.Correo = paciente.correo;
            ViewBag.Telefono = paciente.telefono;
            ViewBag.Pass = paciente.passwd;

            return View("EditarPaciente");
        }

        [HttpPost]
        public ActionResult EditarPaciente(Cliente cliente)
        {

            var clientito = cnx.Clientes.Where(o => o.idCliente == cliente.idCliente).FirstOrDefault();

            clientito.nombres = cliente.nombres;
            clientito.apellidos = cliente.apellidos;
            clientito.dni = cliente.dni;
            clientito.fechaNac = cliente.fechaNac;
            clientito.correo = cliente.correo;
            clientito.telefono = cliente.telefono;
            clientito.passwd = CreateHash(cliente.passwd);
            clientito.idCliente = cliente.idCliente;

            cnx.SaveChanges();

            return RedirectToAction("ClientePerfil");
        }

    }
}
