using Microsoft.AspNetCore.Mvc;
using System;
using Trace.Data;

using Microsoft.EntityFrameworkCore;
namespace Trace.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Roles
                .OrderBy(r => r.Title)
                .ToListAsync());
        }
    }

}
