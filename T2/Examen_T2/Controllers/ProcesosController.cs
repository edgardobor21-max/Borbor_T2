using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Examen_T2.Controllers
{
    public class ProcesosController : Controller
    {
        private int CantidadDivisores(int numero)
        {
            int cantidad = 0;
            for(int i = 1; i <= numero; i++)
            {
                if (numero % i == 0)
                {
                    cantidad++;
                }
            }
            return cantidad;
        }

        private int SumaDivisores(int numero)
        {
            int suma = 0;
            for (int i = 1; i <= numero; i++) {
                if (numero % i == 0)
                {
                    suma += i;
                }
            }
            return suma;
        }

        private int SumaDigitos(int numero)
        {
            int suma = 0;
            while (numero > 0)
            {
                int ultimoDigito = numero % 10;
                suma += ultimoDigito;
                numero = numero / 10;
            }
            return suma;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Operaciones(int numero = 0)
        {
            ViewBag.CantidadDividores = await Task.Run(() => CantidadDivisores(numero)); //1ero ejecutar - 1ero en acabar
            ViewBag.SumaDivisores = await Task.Run(() => SumaDivisores(numero)); //2do en ejecutar - 2do en acabar
            ViewBag.SumaDigitos = await Task.Run(() => SumaDigitos(numero)); //3ero en ejecutar - 3ro en acabar
            return View();
        }

        private readonly HttpClient httpClient = new HttpClient();

        private async Task<string> ObtenerPokemon(int id)
        {
            string url = $"https://pokeapi.co/api/v2/pokemon/{id}"; //Es la URL
            HttpResponseMessage response = await httpClient.GetAsync(url); // Es para recepcionar la respuesta de las peticiones de tipo GET
            response.EnsureSuccessStatusCode(); //Busco un codigo 200 - Se realizo la peticion de manera exitosa
            string json = await response.Content.ReadAsStringAsync(); //Recupera el contenido como texto
            JObject pokemon = JObject.Parse(json); //Convierte el texto a JSON
            return pokemon["name"].ToString(); //Recupera el propiedad name
        }

        public async Task<ActionResult> PokemonSecuencial()
        {
            List<string> pokemones = new List<string>();
            for(int i = 1; i <= 151; i++)
            {
                string pokemon = await ObtenerPokemon(i);
                pokemones.Add(pokemon);
            }
            ViewBag.Pokemones = pokemones;
            return View();
        }

        public async Task<ActionResult> PokemonAsincrono()
        {
            List<Task<string>> tareas = new List<Task<string>>();
            for(int i = 1; i <= 151; i++)
            {
                tareas.Add(ObtenerPokemon(i));
            }
            string[] pokemones = await Task.WhenAll(tareas);
            ViewBag.Pokemones = pokemones;
            return View();
        }
    }
}