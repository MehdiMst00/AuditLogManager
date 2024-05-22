namespace AuditLogManager.Controllers;

public class AuditLogController(AuditLogQueue auditLogQueue) : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] CreateAuditLogDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        auditLogQueue.Enqueue(new AuditLog());

        return NoContent();
    }
}
