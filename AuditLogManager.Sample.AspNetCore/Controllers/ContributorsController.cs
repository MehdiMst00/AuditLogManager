using AuditLogManager.Sample.AspNetCore.Data;
using AuditLogManager.Sample.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditLogManager.Sample.AspNetCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContributorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContributorsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Contributors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contributor>>> GetContributor()
    {
        return await _context.Contributors.ToListAsync();
    }

    // GET: api/Contributors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Contributor>> GetContributor(Guid id)
    {
        var contributor = await _context.Contributors.FindAsync(id);

        if (contributor == null)
        {
            return NotFound();
        }

        return contributor;
    }

    // PUT: api/Contributors/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutContributor(Guid id, Contributor contributor)
    {
        if (id != contributor.Id)
        {
            return BadRequest();
        }

        _context.Entry(contributor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContributorExists(id))
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

    // POST: api/Contributors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Contributor>> PostContributor(Contributor contributor)
    {
        contributor.Id = Guid.NewGuid();
        _context.Contributors.Add(contributor);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetContributor", new { id = contributor.Id }, contributor);
    }

    // DELETE: api/Contributors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContributor(Guid id)
    {
        var contributor = await _context.Contributors.FindAsync(id);
        if (contributor == null)
        {
            return NotFound();
        }

        _context.Contributors.Remove(contributor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ContributorExists(Guid id)
    {
        return _context.Contributors.Any(e => e.Id == id);
    }
}
