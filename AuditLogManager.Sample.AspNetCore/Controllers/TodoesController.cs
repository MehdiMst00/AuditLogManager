using AuditLogManager.Sample.AspNetCore.AuditLogging;
using AuditLogManager.Sample.AspNetCore.Data;
using AuditLogManager.Sample.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditLogManager.Sample.AspNetCore.Controllers;

[Route("api/[controller]")]
[ApiController]
// [ServiceFilter(typeof(AuditLogFilterAttribute))] // Or add globally in startup
public class TodoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TodoesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Todoes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodo()
    {
        return await _context.Todo.ToListAsync();
    }

    // GET: api/Todoes/5
    [HttpGet("{id}")]
    [DisableAuditLogFilter]
    public async Task<ActionResult<Todo>> GetTodo(int id)
    {
        var todo = await _context.Todo.FindAsync(id);

        if (todo == null)
        {
            return NotFound();
        }

        return todo;
    }

    // PUT: api/Todoes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodo(int id, Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest();
        }

        _context.Entry(todo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Todoes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Todo>> PostTodo(Todo todo)
    {
        _context.Todo.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
    }

    // DELETE: api/Todoes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todo.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        _context.Todo.Remove(todo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoExists(int id)
    {
        return _context.Todo.Any(e => e.Id == id);
    }
}
