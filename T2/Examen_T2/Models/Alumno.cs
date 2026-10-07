using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examen_T2.Models
{
    public class Alumno
    {
        public string dni { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string carrera { get; set; }
        public int ciclo { get; set; }

        public Alumno() { }

        public Alumno(string dni, string nombres, string apellidos, string carrera, int ciclo)
        {
            this.dni = dni;
            this.nombres = nombres;
            this.apellidos = apellidos;
            this.carrera = carrera;
            this.ciclo = ciclo;
        }
    }
}
