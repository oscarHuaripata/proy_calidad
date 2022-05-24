using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentoWebMVC.Models.DentoWeb
{
    public class Historia
    {
        public int idHistoria { get; set; }
        public int idCita { get; set; }
        public string observacion { get; set; }
        public string motivo { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public string examenes { get; set; }
        public string diagnostico { get; set; }
        public string tratamiento { get; set; }



        public Cita cita { get; set; }



    }
}
