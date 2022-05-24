using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DentoWebMVC.Models.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DentoWebMVC.Controllers
{
    
    public class AuthController : Controller
    {
        DentoWebContext cnx;

        public readonly IConfiguration configuration;


        public AuthController(DentoWebContext cnx, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.cnx = cnx;
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string passwd)
        {
            var cliente = cnx.Clientes.Where(o => o.usuario == username && CreateHash(passwd) == o.passwd).FirstOrDefault();
            if (cliente != null)
            {
                // Autenticaremos
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("Index", "Cliente");
            }

            return View();
        }

        public ActionResult LoginSeparar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginSeparar(string username, string passwd)
        {
            var cliente = cnx.Clientes.Where(o => o.usuario == username && CreateHash(passwd) == o.passwd).FirstOrDefault();
            if (cliente != null)
            {
                // Autenticaremos
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("CreateCita", "Cliente");
            }

            return View();
        }

        private string CreateHash(string input)
        {
            var sha = SHA256.Create();
            input += configuration.GetValue<string>("Token");
            var hash = sha.ComputeHash(Encoding.Default.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        public ActionResult LoginDoctor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginDoctor(string username, string passwd)
        {
            var doctor = cnx.Doctors.Where(o => o.usuario == username && CreateHash(passwd) == o.passwd).FirstOrDefault();
            if (doctor != null)
            {
                // Autenticaremos
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("Index", "Doctor");
            }

            return View();
        }

    }
}
