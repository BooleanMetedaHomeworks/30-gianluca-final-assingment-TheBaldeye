using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Models;

namespace PizzaWebApi.Controllers
{
    // SEZIONE COMPLETA (30%)
    // Questo è un Controller API che gestisce tutte le operazioni relative alle categorie
    [ApiController]  // Indica che questa classe è un controller API
    [Route("[controller]")]  // Il routing sarà basato sul nome del controller (es: /Category/...)
    [Authorize]  // Richiede che l'utente sia autenticato per accedere a questi endpoint
    public class CategoryController : ControllerBase
    {
        // Repository è la classe che si occupa di accedere ai dati nel database
        private readonly CategoryRepository _categoryRepo;

        // Il costruttore riceve il repository attraverso la dependency injection
        public CategoryController(CategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // Endpoint GET che restituisce tutte le categorie o filtra per nome
        [HttpGet]
        [Authorize(Roles = "REGULAR,SUPERVISOR,ADMIN")]
        public async Task<IActionResult> Get(string? name)
        {
            try
            {
                if (name != null)
                    return Ok(await _categoryRepo.GetCategoriesByName(name));
                return Ok(await _categoryRepo.GetAllCategories());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // QUIZ 1 (25%): Come implementeresti l'endpoint per ottenere una categoria per ID?
        // Obiettivo: Creare un endpoint GET che accetta un ID e restituisce la categoria corrispondente
        // Processo logico:
        // 1. L'endpoint deve rispondere a GET /Category/{id}
        // 2. Deve gestire il caso in cui la categoria non esiste
        // 3. Deve gestire eventuali errori

        /* SCEGLI TRA:
        A)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepo.GetCategoryById(id);
            return Ok(category);
        }*/

        //B)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryRepo.GetCategoryById(id);
                if (category == null)
                    return NotFound();
                return Ok(category);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*C)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_categoryRepo.GetCategoryById(id));
        }
        */

        // QUIZ 2 (25%): Implementazione dell'endpoint CREATE
        // Obiettivo: Creare un endpoint POST per inserire una nuova categoria
        // Processo logico:
        // 1. Solo SUPERVISOR e ADMIN possono creare categorie
        // 2. Validare il modello ricevuto
        // 3. Assicurarsi che l'ID sia 0 per una nuova categoria

        /* SCEGLI TRA:
        A)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category newCategory)
        {
            return Ok(await _categoryRepo.InsertCategory(newCategory));
        }*/

        //B)
        [HttpPost]
        [Authorize(Roles = "SUPERVISOR,ADMIN")]
        public async Task<IActionResult> Create([FromBody] Category newCategory)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return BadRequest(ModelState.Values);
                newCategory.Id = 0;
                var affectedRows = await _categoryRepo.InsertCategory(newCategory);
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
        public async Task<IActionResult> Create(Category newCategory)
        {
            await _categoryRepo.InsertCategory(newCategory);
            return Ok();
        }
        */

        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare l'endpoint DELETE
        // Tips:
        // 1. Usa l'attributo HttpDelete e il routing con l'ID
        // 2. Solo gli ADMIN possono eliminare categorie
        // 3. Gestisci il caso in cui la categoria non esiste
        // 4. Gestisci eventuali errori con try-catch
        // 5. Usa NotFound() se la categoria non esiste

        // Il tuo codice qui...
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                var category = await _categoryRepo.DeleteCategoryById(id);
                if (category == null)
                    return NotFound();
                return Ok(category);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }       
    }
}