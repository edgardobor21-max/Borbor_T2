using Newtonsoft.Json;
using Examen_T2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examen_T2.Controllers
{
    public class AlumnoController : Controller
    {
        // Lista inicialmente vacia, almacenada como string en formato JSON.
        static string jlistAlumnos = @"[]";

        public ActionResult Index()
        {
            try
            {
                List<Alumno> listIntermedia = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
                return View(listIntermedia);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View(new List<Alumno>());
            }
        }

        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Agregar(Alumno alumno)
        {
            if (!ModelState.IsValid) return View(alumno);
            if (string.IsNullOrWhiteSpace(alumno.dni) ||
                string.IsNullOrWhiteSpace(alumno.nombres) ||
                string.IsNullOrWhiteSpace(alumno.apellidos) ||
                string.IsNullOrWhiteSpace(alumno.carrera) || alumno.ciclo <= 0)
            {
                ViewBag.Mensaje = "Complete todos los datos y escriba un ciclo mayor que cero";
                return View(alumno);
            }
            alumno.dni = alumno.dni.Trim();
            try
            {
                // 1. Recuperar el string jlistAlumnos y deserializarlo.
                List<Alumno> temporal = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
                // 2. Buscar el alumno por su DNI para evitar duplicados.
                Alumno alumnoEncontrado = temporal.Find(a => a.dni == alumno.dni);
                if (alumnoEncontrado != null)
                {
                    ViewBag.Mensaje = "El DNI ya existe en la coleccion";
                    return View(alumno);
                }
                // 3. Instanciar la clase y agregar el alumno a la lista.
                Alumno alumnoNuevo = new Alumno(alumno.dni, alumno.nombres,
                    alumno.apellidos, alumno.carrera, alumno.ciclo);
                temporal.Add(alumnoNuevo);
                // 4. Serializar la lista y sobreescribir jlistAlumnos.
                jlistAlumnos = JsonConvert.SerializeObject(temporal);
                ViewBag.Mensaje = "Guardado de manera exitosa";
                return View(alumno);
            }
            catch (JsonException ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View(alumno);
            }
        }


        public ActionResult Detalles(string dni)
        {
            // Deserializar y buscar el objeto por su DNI.
            List<Alumno> temporal = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
            Alumno alumnoEncontrado = temporal.Find(a => a.dni == dni);
            if (alumnoEncontrado == null) return HttpNotFound("Alumno no encontrado");
            return View(alumnoEncontrado);
        }

        public ActionResult Actualizar(string dni)
        {
            // 1. Deserializar el string jlistAlumnos.
            List<Alumno> temporal = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
            // 2. Buscar el objeto en la lista.
            Alumno alumnoEncontrado = temporal.Find(a => a.dni == dni);
            if (alumnoEncontrado == null) return HttpNotFound("Alumno no encontrado");
            // Conservar el DNI original para localizar al alumno aunque se cambie el DNI.
            ViewBag.dniOriginal = alumnoEncontrado.dni;
            return View(alumnoEncontrado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Actualizar(Alumno alumno, string dniOriginal)
        {
            ViewBag.dniOriginal = dniOriginal;
            if (!ModelState.IsValid) return View(alumno);
            if (string.IsNullOrWhiteSpace(dniOriginal))
            {
                ViewBag.Mensaje = "No se recibio el DNI original del alumno";
                return View(alumno);
            }
            if (string.IsNullOrWhiteSpace(alumno.dni) ||
                string.IsNullOrWhiteSpace(alumno.nombres) ||
                string.IsNullOrWhiteSpace(alumno.apellidos) ||
                string.IsNullOrWhiteSpace(alumno.carrera) || alumno.ciclo <= 0)
            {
                ViewBag.Mensaje = "Complete todos los datos y escriba un ciclo mayor que cero";
                return View(alumno);
            }
            alumno.dni = alumno.dni.Trim();
            try
            {
                // 1. Deserializar el string jlistAlumnos.
                List<Alumno> temporal = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
                // 2. Buscar en temporal el elemento por su DNI original.
                int index = temporal.FindIndex(a => a.dni == dniOriginal);
                // 3. Verificar que el nuevo DNI no corresponda a otro alumno.
                int indexRepetido = temporal.FindIndex(a => a.dni == alumno.dni);
                if (indexRepetido != -1 && indexRepetido != index)
                {
                    ViewBag.Mensaje = "El DNI ya existe en la coleccion";
                    return View(alumno);
                }
                // 4. Instanciar la clase con los datos del formulario.
                Alumno alumnoNuevo = new Alumno(alumno.dni, alumno.nombres,
                    alumno.apellidos, alumno.carrera, alumno.ciclo);
                if (index != -1)
                {
                    // Actualizar la misma posicion, sin duplicar el registro.
                    temporal[index] = alumnoNuevo;
                    ViewBag.Mensaje = "Alumno actualizado";
                }
                else
                {
                    // Condicion indicada en el enunciado: si no existe, agregarlo.
                    temporal.Add(alumnoNuevo);
                    ViewBag.Mensaje = "Alumno agregado porque el DNI original no existia";
                }
                // 5. Serializar de List<Alumno> a string jlistAlumnos.
                jlistAlumnos = JsonConvert.SerializeObject(temporal);
                // Permitir otra actualizacion aun si se modifico el DNI.
                ViewBag.dniOriginal = alumnoNuevo.dni;
                ModelState.Clear();
                return View(alumnoNuevo);
            }
            catch (JsonException ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View(alumno);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(string dni)
        {
            // 1. Deserializar el string jlistAlumnos.
            List<Alumno> temporal = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
            // 2. Buscar el objeto por su DNI.
            Alumno alumnoEncontrado = temporal.Find(a => a.dni == dni);
            // 3. Eliminar el objeto encontrado de la lista.
            if (alumnoEncontrado != null)
            {
                temporal.Remove(alumnoEncontrado);
            }
            // 4. Serializar la lista actualizada y redireccionar a Index.
            jlistAlumnos = JsonConvert.SerializeObject(temporal);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Serializar()
        {
            string mensaje = string.Empty;
            try
            {
                List<Alumno> temporal = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
                string contenido = JsonConvert.SerializeObject(temporal);
                string ruta = Server.MapPath("~/App_Data/alumnos.json");
                System.IO.File.WriteAllText(ruta, contenido);
                mensaje = "Serializado de manera correcta. Archivo guardado en App_Data/alumnos.json";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            ViewBag.Mensaje = mensaje;
            List<Alumno> listIntermedia = JsonConvert.DeserializeObject<List<Alumno>>(jlistAlumnos);
            return View("Index", listIntermedia);
        }
    }
}
