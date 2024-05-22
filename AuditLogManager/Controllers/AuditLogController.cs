namespace AuditLogManager.Controllers;

public class AuditLogController(AuditLogQueue auditLogQueue, 
    IGuidGenerator guidGenerator) : BaseApiController
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

        auditLogQueue.Enqueue(dto.ToAudit(guidGenerator));

        return NoContent();
    }
}