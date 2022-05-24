using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentoWebMVC.Models.DentoWeb
{
    public class Doctor
    {
        public int idDoctor { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string codigoCol { get; set; }
        public string casaEstudio { get; set; }
        public string titulo { get; set; }
        public string dni { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string usuario { get; set; }
        public string passwd { get; set; }


        public List<Cita> Citas { get; set; }
    }
}
