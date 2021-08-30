using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NoticiasTiempo;
using NoticiasTiempo.Models;

namespace NoticiasTiempo.Controllers
{
    [AllowCrossSiteJson]
    public class GeneralController : ApiController
    {
        private NoticiasBDEntities db = new NoticiasBDEntities();
        #region Historial
        [HttpGet]
        [Route("api/GetHistorial")]
        [ResponseType(typeof(Historial))]
        public IHttpActionResult GetHistorial()
        {
            var list = (from e in db.Historial select e).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Route("api/GetHistorial/{ciudad}")]
        [ResponseType(typeof(Historial))]
        public IHttpActionResult GetHistorial(string Ciudad)
        {
            var historial = (from e in db.Historial select e).Where(x => x.nombciudad == Ciudad).ToList();
            if (historial == null)
            {
                return NotFound();
            }

            return Ok(historial);
        }

        
        [Route("api/AgregarHistorial/{ciudad}")]
        [ResponseType(typeof(Historial))]
        public IHttpActionResult AgregarHistorial(string Ciudad)
        {
            Historial historial = new Historial();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                historial.nombciudad = Ciudad;
                historial.info = "Se ha realizado busqueda de " + Ciudad + " el dia " + DateTime.Today.ToString("dd/MM/yyy");
                db.Historial.Add(historial);
                db.SaveChanges();
                return Ok("Datos de Reserva Registrados");
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        #endregion Historial

        #region GENERAL
        [HttpGet]
        [Route("api/GetGeneralRespuesta/{ciudad}")]
        public IHttpActionResult GetGeneralRespuesta(string ciudad)
        {
            #region tiempo
            //instancia para invocar el servicio tipo REST para el tiempo
            HttpClient Time = new HttpClient();

            //ENDOPOINT del estado del tiempo
            Time.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/weather");
            Time.DefaultRequestHeaders.Accept.Clear();
            Time.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //variables de busqueda el cial ?q corresponde a la ciudad en el endpoint
            string ciudad_q = "?q=";
            //key de la api para poder realizar consultas
            string key = "&appid=2b4400b1f69af9df341160d7be8fcff8";

            //concatenamos todo el endpoint
            var ConcaEndpoint = String.Concat(ciudad_q, ciudad, key);

            //generamos la peticion GET
            var request = Time.GetAsync(ConcaEndpoint).Result;
            #endregion
            #region noticia
            //instancia para invocar el servicio tipo REST para las noticias
            HttpClient news_s = new HttpClient();

            //ENDOPOINT noticias
            news_s.BaseAddress = new Uri("https://newsapi.org/v2/top-headlines");
            news_s.DefaultRequestHeaders.Accept.Clear();
            news_s.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //valor a consultar por pais en la api noticias
            string country_n = "?country=";
            //key de la api noticias
            string key_noticia = "&apiKey=48238d7c4eb34e11a50b32c0c41db15b";
            #endregion

            if (request.IsSuccessStatusCode)
            {
                tiempoModel tiempoEST = request.Content.ReadAsAsync<tiempoModel>().Result;

                //concatenamos todo el endpoint
                var ConcaEndpoint_n = String.Concat(country_n, tiempoEST.sys.country, key_noticia);
                //generamos la peticion GET
                var request_n = news_s.GetAsync(ConcaEndpoint_n).Result;
                //procedemos a leer la respuesta
                NoticiaModel noticias = request_n.Content.ReadAsAsync<NoticiaModel>().Result;

                //se crear un diccionario de objetos para tener todos los datos en uno solo y mandarlo de respuesta
                var DatosResponse = new Dictionary<string, object>();
                DatosResponse.Add("Noticia", noticias);
                DatosResponse.Add("EstadoTiempo", tiempoEST);

                //creamos el insert de los datos  en el historial
                this.AgregarHistorial(ciudad);

                return Ok(DatosResponse);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion GENERAL

    }
}
