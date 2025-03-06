using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Models;

namespace PizzaWebApi.Controllers
{
    // SEZIONE COMPLETA (30%)
    // ========== CONTROLLER DELLE PIZZE ==========
    // Questo controller gestisce tutte le operazioni CRUD (Create, Read, Update, Delete) relative alle pizze
    // Ogni endpoint � protetto da autenticazione, quindi solo gli utenti registrati possono accedervi
    // Inoltre, alcuni endpoint sono ulteriormente protetti da autorizzazione basata su ruoli
    [ApiController]  // Indica che questa classe � un controller API e abilita automaticamente alcune funzionalit�
    [Route("[controller]")]  // Definisce il percorso base per tutte le richieste (es: /Pizza/...)
    [Authorize]  // Richiede che l'utente sia autenticato per TUTTI gli endpoint di questo controller
    public class PizzaController : ControllerBase
    {
        // Il repository � la classe che si occupa di tutte le operazioni sul database
        // Lo manteniamo privato (private readonly) per sicurezza e per evitare modifiche accidentali
        private readonly PizzaRepository _pizzaRepository;

        // Il costruttore riceve il repository tramite dependency injection
        // Questo significa che non creiamo noi l'istanza del repository, ma la riceviamo dal sistema
        // Questo approccio rende il codice pi� testabile e manutenibile
        public PizzaController(PizzaRepository pizzaRepository)
        {
            _pizzaRepository = pizzaRepository;
        }

        // Endpoint GET che pu�:
        // 1. Restituire TUTTE le pizze se non viene specificato un nome
        // 2. Filtrare le pizze per nome se viene fornito il parametro 'name'
        // � accessibile a utenti REGULAR, SUPERVISOR e ADMIN
        [HttpGet]  // Risponde alle richieste HTTP GET
        [Authorize(Roles = "REGULAR,SUPERVISOR,ADMIN")]  // Specifica quali ruoli possono accedere
        public async Task<IActionResult> Get(string? name)  // Il '?' indica che name � opzionale
        {
            try
            {
                // Se viene fornito un nome, filtriamo le pizze
                if (name != null)
                    return Ok(await _pizzaRepository.GetPizzasByName(name));
                // Altrimenti restituiamo tutte le pizze
                return Ok(await _pizzaRepository.GetAllPizzas());
            }
            catch (Exception e)
            {
                // In caso di errore, restituiamo un BadRequest con il messaggio di errore
                // Questo aiuta il client a capire cosa � andato storto
                return BadRequest(e.Message);
            }
        }

        // QUIZ 1 (25%): Come implementeresti l'endpoint per creare una nuova pizza?
        // Obiettivo: Implementare un endpoint POST sicuro per inserire una nuova pizza
        // Processo logico:
        // 1. Solo SUPERVISOR e ADMIN possono creare pizze (sicurezza basata su ruoli)
        // 2. Il modello ricevuto deve essere validato per evitare dati incorretti
        // 3. L'ID deve essere 0 per assicurarsi che venga creato un nuovo record
        // 4. Gli errori devono essere gestiti appropriatamente

        /* SCEGLI TRA:
        A)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Pizza newPizza)
        {
            return Ok(await _pizzaRepository.InsertPizza(newPizza));
        }*/

        //B)
        [HttpPost]
        [Authorize(Roles = "SUPERVISOR,ADMIN")]
        public async Task<IActionResult> Create([FromBody] Pizza newPizza)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                newPizza.Id = 0;
                var affectedRows = await _pizzaRepository.InsertPizza(newPizza);
                return Ok(affectedRows);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*C)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Pizza newPizza)
        {
            if(newPizza == null)
                return BadRequest();
            await _pizzaRepository.InsertPizza(newPizza);
            return Ok();
        }
        */

        // QUIZ 2 (25%): Come implementeresti l'endpoint di aggiornamento?
        // Obiettivo: Creare un endpoint PUT sicuro per modificare una pizza esistente
        // Processo logico:
        // 1. Solo gli ADMIN possono modificare le pizze (massima sicurezza)
        // 2. Il modello deve essere validato
        // 3. Dobbiamo gestire il caso in cui la pizza da modificare non esiste
        // 4. Tutti gli errori devono essere gestiti

        /* SCEGLI TRA:
        A)*/
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] Pizza newPizza)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                var affectedRows = await _pizzaRepository.UpdatePizza(id, newPizza);
                if (affectedRows == 0)
                {
                    return NotFound();
                }
                return Ok(affectedRows);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*B)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Pizza newPizza)
        {
            await _pizzaRepository.UpdatePizza(id, newPizza);
            return Ok();
        }

        C)
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update([FromBody] Pizza newPizza)
        {
            return Ok(await _pizzaRepository.UpdatePizza(newPizza.Id, newPizza));
        }
        */

        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare l'endpoint DELETE per eliminare una pizza
        // Questa operazione � delicata perch�:
        // - Elimina permanentemente dati dal sistema
        // - Deve essere limitata solo agli amministratori
        // - Deve gestire correttamente il caso di pizza non trovata
        // 
        // Tips per l'implementazione:
        // 1. Usa [HttpDelete("{id}")] per il routing
        //    Questo permette di chiamare l'endpoint con DELETE /Pizza/5
        // 
        // 2. Proteggi l'endpoint con [Authorize(Roles = "ADMIN")]
        //    Solo gli amministratori possono eliminare le pizze
        // 
        // 3. La firma del metodo deve essere:
        //    public async Task<IActionResult> Delete(int id)
        // 
        // 4. Usa un blocco try-catch per gestire gli errori
        //    try {
        //        // codice che potrebbe generare errori
        //    }
        //    catch (Exception e) {
        //        return BadRequest(e.Message);
        //    }
        // 
        // 5. Usa il repository per eliminare la pizza:
        //    var affectedRows = await _pizzaRepository.DeletePizza(id);
        // 
        // 6. Se affectedRows � 0, significa che la pizza non esisteva
        //    In questo caso, restituisci NotFound()

        // Il tuo codice qui...
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var affectedRows = await _pizzaRepository.DeletePizza(id);
                if (affectedRows == 0)
                    return NotFound();
                return Ok(new { DeletedPizzas = affectedRows });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }   
    }
}