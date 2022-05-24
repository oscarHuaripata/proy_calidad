using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DentoWebMVC.Models.Context;
using DentoWebMVC.Models.DentoWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;

namespace DentoWebMVC.Controllers
{
    public class DoctorController : Controller
    {
        public DentoWebContext cnx;
        public readonly IConfiguration configuration;
        public DoctorController(DentoWebContext cnx,IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cnx = cnx;
        }


        public IActionResult Index()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = cnx.Doctors.Where(o => o.usuario == claim.Value).FirstOrDefault();
            ViewBag.User = user;

            var citas = cnx.Citas.Include(o => o.horario).Include(o => o.cliente).Where(o => o.idDoctor == user.idDoctor).ToList();

            ViewBag.Citas = citas;

            return View();
        }

        public ActionResult CancelarCita(int id)
        {
            var cita = cnx.Citas.Where(o => o.idCita == id).FirstOrDefault();

            cita.estado = "CANCELADA";

            cnx.SaveChanges();


            return RedirectToAction("Index", "Doctor");
        }

        [HttpGet]
        public ActionResult AtenderCita(string id)
        {
            ViewBag.idCita = id;
            return View();
        }
        [HttpPost]
        public ActionResult AtenderCita(int idCita,string observacion,string motivo,string descripcion,
            string examenes,string diagnostico,string tratamiento)
        {

            Cita cita = cnx.Citas.Where(o => o.idCita == idCita).FirstOrDefault();

            cita.estado = "ATENDIDO";


            Historia historia = new Historia();
            historia.idCita = idCita;
            historia.observacion = observacion;
            historia.motivo = motivo;
            historia.fecha = DateTime.Now.Date;
            historia.descripcion = descripcion;
            historia.examenes = examenes;
            historia.diagnostico = diagnostico;
            historia.tratamiento = tratamiento;

            cnx.Historias.Add(historia);
            cnx.SaveChanges();

            return RedirectToAction("Index","Doctor");
        }


        public ActionResult DetalleCita(int id)
        {
            var historia = cnx.Historias.Include(o => o.cita).Where(o => o.idCita == id).FirstOrDefault();
            var paciente = cnx.Clientes.Where(o => o.idCliente == historia.cita.idCliente).FirstOrDefault();
            var doctor = cnx.Doctors.Where(o => o.idDoctor == historia.cita.idDoctor).FirstOrDefault();
            ViewBag.Doctor = doctor;
            ViewBag.Historia = historia;
            ViewBag.Paciente = paciente;
            return View();
        }


        public ActionResult ListaClientes()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = cnx.Doctors.Where(o => o.usuario == claim.Value).FirstOrDefault();
            ViewBag.User = user;
            var clientes = cnx.Clientes.ToList();
            ViewBag.Clientes = clientes;
            return View();
        }

        public ActionResult ListarHistorial(int id)
        {
            var historial = cnx.Historias.Include(o => o.cita).Where(o => o.cita.idCliente == id).ToList();
            ViewBag.Horarios = cnx.Horarios.ToList();
            ViewBag.Historias = historial;

            ViewBag.Doctors = cnx.Doctors.ToList();
            return View();
        }

        public ActionResult DoctorPerfil()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = cnx.Doctors.Where(o => o.usuario == claim.Value).FirstOrDefault();
            ViewBag.Id = user.idDoctor;
           
            ViewBag.User = user;

            return View();
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

            return View("IndexBoletaDoctor");
        }

        private string CreateHash(string input)
        {
            var sha = SHA256.Create();
            input += configuration.GetValue<string>("Token");
            var hash = sha.ComputeHash(Encoding.Default.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        [HttpGet]
        public ActionResult EditarDoctor(int id)
        {
            ViewBag.Id = id;
            var d = cnx.Doctors.Where(o => o.idDoctor == id).FirstOrDefault();

            ViewBag.Nombres = d.nombres;
            ViewBag.Apellidos = d.apellidos;
            ViewBag.Codigo = d.codigoCol;
            ViewBag.Casa = d.casaEstudio;
            ViewBag.Titulo = d.titulo;
            ViewBag.Dni = d.dni;
            ViewBag.Correo = d.correo;
            ViewBag.Telefono = d.telefono;
            ViewBag.Pass = d.passwd;

            return View("EditarDoctor");
        }

        [HttpPost]
        public ActionResult EditarDoctor(Doctor doctor)
        {

            var doc = cnx.Doctors.Where(o => o.idDoctor == doctor.idDoctor).FirstOrDefault();

            doc.nombres = doctor.nombres;
            doc.apellidos = doctor.apellidos;
            doc.dni = doctor.codigoCol;
            doc.casaEstudio = doctor.casaEstudio;
            doc.titulo = doctor.titulo;
            doc.dni = doctor.dni;
            doc.correo = doctor.correo;
            doc.telefono = doctor.telefono;
            doc.passwd = CreateHash(doctor.passwd);
            doc.idDoctor = doctor.idDoctor;

            cnx.SaveChanges();

            return RedirectToAction("DoctorPerfil");
        }

    }
}
