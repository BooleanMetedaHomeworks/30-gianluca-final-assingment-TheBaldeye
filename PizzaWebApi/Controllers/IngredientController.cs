using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Models;

namespace PizzaWebApi.Controllers
{
    // SEZIONE COMPLETA (30%)
    // ========== CONTROLLER DEGLI INGREDIENTI ==========
    // Questo controller gestisce tutte le operazioni CRUD (Create, Read, Update, Delete) per gli ingredienti
    // È fondamentale per la gestione del menu, permettendo di:
    // - Visualizzare tutti gli ingredienti disponibili
    // - Aggiungere nuovi ingredienti
    // - Modificare ingredienti esistenti
    // - Rimuovere ingredienti non più utilizzati
    [ApiController]  // Abilita comportamenti specifici per le API (come la validazione automatica del modello)
    [Route("[controller]")]  // Il routing sarà basato sul nome del controller, es: /Ingredient/...
    //[Authorize]  // Tutti gli endpoint richiedono un utente autenticato
    public class IngredientController : ControllerBase
    {
        // Il repository gestisce tutte le operazioni sul database degli ingredienti
        // È dichiarato readonly per evitare modifiche accidentali dopo l'inizializzazione
        private readonly IngredientRepository _ingredientRepository;

        // Il costruttore riceve il repository tramite dependency injection
        // ASP.NET Core si occupa automaticamente di:
        // 1. Creare un'istanza del repository
        // 2. Passarla al controller quando viene creato
        // 3. Gestire il ciclo di vita dell'oggetto
        public IngredientController(IngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        // Endpoint GET principale che restituisce tutti gli ingredienti
        // È accessibile a utenti REGULAR, SUPERVISOR e ADMIN
        // Viene chiamato quando si fa una richiesta GET a /Ingredient
        [HttpGet]
        //[Authorize(Roles = "REGULAR,SUPERVISOR,ADMIN")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Recupera tutti gli ingredienti dal database
                // await indica che l'operazione è asincrona (non blocca il thread)
                var ingredients = await _ingredientRepository.GetAllIngredients();

                // Restituisce la lista con status code 200 (OK)
                return Ok(ingredients);
            }
            catch (Exception e)
            {
                // Se si verifica un errore, restituisce status code 400 (Bad Request)
                // Include il messaggio di errore per aiutare il debugging
                return BadRequest(e.Message);
            }
        }

        // QUIZ 1 (25%): Come implementeresti l'endpoint per ottenere un ingrediente specifico?
        // Obiettivo: Creare un endpoint GET che restituisce un singolo ingrediente dato il suo ID
        // Processo logico:
        // 1. L'endpoint deve accettare un ID come parametro nell'URL
        // 2. Deve gestire il caso in cui l'ingrediente non esiste
        // 3. Deve gestire eventuali errori del database

        /* SCEGLI TRA:
        A)
        // Implementazione incompleta:
        // - Non gestisce il caso di ingrediente non trovato
        // - Non gestisce gli errori
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _ingredientRepository.GetIngredientById(id);
            return Ok(ingredient);
        }*/

        //B)
        // Implementazione corretta:
        // - Gestisce il caso di ingrediente non trovato (404)
        // - Gestisce gli errori in modo appropriato
        // - Usa try-catch per la gestione degli errori
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var ingredient = await _ingredientRepository.GetIngredientById(id);
                if (ingredient == null)
                    return NotFound();
                return Ok(ingredient);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*C)
        // Implementazione problematica:
        // - Routing non corretto (manca il parametro nell'URL)
        // - Non gestisce gli errori
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            return Ok(await _ingredientRepository.GetIngredientById(id));
        }
        */

        // QUIZ 2 (25%): Come implementeresti l'endpoint di aggiornamento?
        // Obiettivo: Permettere la modifica di un ingrediente esistente
        // Processo logico:
        // 1. Solo gli ADMIN possono modificare gli ingredienti
        // 2. Validare i dati ricevuti
        // 3. Gestire il caso di ingrediente non trovato

        /*SCEGLI TRA:
        A)*/
        // Implementazione corretta:
        // - Autorizzazione appropriata
        // - Validazione del modello
        // - Gestione completa degli errori
        [HttpPut("{id}")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] Ingredient ingredient)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                var affectedRows = await _ingredientRepository.UpdateIngredient(id, ingredient);
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
        // Implementazione insufficiente:
        // - Manca l'autorizzazione
        // - Non valida i dati
        [HttpPut]
        public async Task<IActionResult> Update(Ingredient ingredient)
        {
            await _ingredientRepository.UpdateIngredient(ingredient.Id, ingredient);
            return Ok();
        }

        C)
        // Implementazione con problemi:
        // - Autorizzazione troppo permissiva
        // - Gestione errori incompleta
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, Ingredient ingredient)
        {
            try
            {
                return Ok(await _ingredientRepository.UpdateIngredient(id, ingredient));
            }
            catch
            {
                return BadRequest();
            }
        }
        */

        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare l'endpoint per la creazione di un nuovo ingrediente
        // È un'operazione delicata perché:
        // - Gli ingredienti sono fondamentali per la composizione delle pizze
        // - Solo utenti autorizzati dovrebbero poter aggiungere ingredienti
        // - I dati devono essere validati per evitare problemi
        //
        // Tips per l'implementazione:
        // 1. Usa [HttpPost] per gestire le richieste POST
        //    Questo endpoint sarà chiamato con POST /Ingredient
        //
        // 2. Limita l'accesso a SUPERVISOR e ADMIN:
        //    [Authorize(Roles = "SUPERVISOR,ADMIN")]
        //
        // 3. Il metodo deve accettare l'ingrediente dal body:
        //    public async Task<IActionResult> Create([FromBody] Ingredient newIngredient)
        //
        // 4. Valida il modello prima di procedere:
        //    if (ModelState.IsValid == false)
        //        return BadRequest(ModelState.Values);
        //
        // 5. Imposta Id = 0 per assicurarti che venga creato un nuovo record:
        //    newIngredient.Id = 0;
        //
        // 6. Usa il repository per salvare l'ingrediente:
        //    var result = await _ingredientRepository.InsertIngredient(newIngredient);
        //
        // 7. Gestisci gli errori con try-catch

        // Il tuo codice qui...
        [HttpPost]
        //[Authorize(Roles = "SUPERVISOR,ADMIN")]
        public async Task<IActionResult> Create([FromBody] Ingredient newIngredient)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState.Values);

                newIngredient.Id = 0;
                var result = await _ingredientRepository.InsertIngredient(newIngredient);
                return Created($"/Ingredient/{newIngredient.Id}", newIngredient);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}