using Microsoft.AspNetCore.Mvc;

namespace PizzaWebApi.Controllers
{
    // SEZIONE COMPLETA (30%)
    // ========== CONTROLLER PUBBLICO ==========
    // Questo controller è speciale perché gestisce gli endpoint pubblici
    // A differenza degli altri controller, questi endpoint sono accessibili
    // senza autenticazione - chiunque può chiamarli
    [ApiController]  // Abilita funzionalità specifiche per le API
    [Route("[controller]")]  // Il routing sarà /Public/...
    public class PublicController : ControllerBase
    {
        // Repository per accedere ai dati delle pizze
        // Viene usato per mostrare un menu limitato al pubblico
        private readonly PizzaRepository _pizzaRepository;

        // Il costruttore riceve il repository tramite dependency injection
        // Anche se è un controller pubblico, manteniamo le best practice
        // di dependency injection per la consistenza e manutenibilità
        public PublicController(PizzaRepository pizzaRepository)
        {
            _pizzaRepository = pizzaRepository;
        }

        // Questo endpoint è l'unico presente nel controller e restituisce
        // una lista limitata di pizze visibili al pubblico
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Nota il parametro 5 passato a GetAllPizzas
                // Questo limita il numero di pizze restituite
                return Ok(await _pizzaRepository.GetAllPizzas(5));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // QUIZ 1 (35%): Come implementeresti un endpoint per ottenere i dettagli di una singola pizza?
        // Obiettivo: Creare un endpoint pubblico per visualizzare una pizza specifica
        // Processo logico:
        // 1. L'endpoint deve accettare un ID
        // 2. Deve gestire il caso di pizza non trovata
        // 3. Non deve richiedere autenticazione

        /* SCEGLI TRA:
        A)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var pizza = _pizzaRepository.GetPizzaById(id);
            return Ok(pizza);
        }

        B)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pizza = await _pizzaRepository.GetPizzaById(id);
                if (pizza == null)
                    return NotFound();
                return Ok(pizza);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        C)
        [HttpGet("GetPizza")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var pizza = await _pizzaRepository.GetPizzaById(id);
            return Ok(pizza);
        }
        */

        // QUIZ 2 (35%): Come implementeresti un endpoint per cercare pizze per nome?
        // Obiettivo: Permettere la ricerca pubblica di pizze per nome
        // Processo logico:
        // 1. Accettare il parametro di ricerca
        // 2. Limitare i risultati per performance
        // 3. Gestire casi di errore

        /* SCEGLI TRA:
        A)
        [HttpGet("search/{name}")]
        public async Task<IActionResult> Search(string name)
        {
            return Ok(await _pizzaRepository.GetPizzasByName(name));
        }

        B)
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            try
            {
                var pizzas = await _pizzaRepository.GetPizzasByName(name);
                return Ok(pizzas.Take(5));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        C)
        [HttpGet("search")]
        public IActionResult Search(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();
            return Ok(_pizzaRepository.GetPizzasByName(name).Result);
        }
        */
    }
}