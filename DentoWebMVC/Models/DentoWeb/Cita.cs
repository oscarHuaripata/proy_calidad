using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentoWebMVC.Models.DentoWeb
{
    public class Cita
    {
        public int idCita { get; set; }
        public DateTime fecha { get; set; }
        public int idHorario { get; set; }
        public string estado { get; set; }
        public string pago { get; set; }
        public int idCliente { get; set; }
        public int idDoctor { get; set; }
        public decimal monto { get; set; }

        public Cliente cliente { get; set; }
        public Doctor doctor { get; set; }
        public Horario horario { get; set; }

    }
}
