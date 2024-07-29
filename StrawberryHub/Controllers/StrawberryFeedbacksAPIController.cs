using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using StrawberryHub.Services;

namespace StrawberryHub
{
    [Route("api/[controller]")]
    [ApiController]
    public class StrawberryFeedbacksAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StrawberryFeedbacksAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StrawberryFeedbacksAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryFeedback>>> GetStrawberryFeedback()
        {
          if (_context.StrawberryFeedback == null)
          {
              return NotFound();
          }
            return await _context.StrawberryFeedback.ToListAsync();
        }

        // GET: api/StrawberryFeedbacksAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryFeedback>> GetStrawberryFeedback(int id)
        {
          if (_context.StrawberryFeedback == null)
          {
              return NotFound();
          }
            var strawberryFeedback = await _context.StrawberryFeedback.FindAsync(id);

            if (strawberryFeedback == null)
            {
                return NotFound();
            }

            return strawberryFeedback;
        }

        // PUT: api/StrawberryFeedbacksAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStrawberryFeedback(int id, StrawberryFeedback strawberryFeedback)
        {
            if (id != strawberryFeedback.FeedbackId)
            {
                return BadRequest();
            }

            _context.Entry(strawberryFeedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StrawberryFeedbackExists(id))
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

        // POST: api/StrawberryFeedbacksAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StrawberryFeedback>> PostStrawberryFeedback(StrawberryFeedback strawberryFeedback)
        {
          if (_context.StrawberryFeedback == null)
          {
              return Problem("Entity set 'AppDbContext.StrawberryFeedback'  is null.");
          }
            _context.StrawberryFeedback.Add(strawberryFeedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStrawberryFeedback", new { id = strawberryFeedback.FeedbackId }, strawberryFeedback);
        }

        // DELETE: api/StrawberryFeedbacksAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrawberryFeedback(int id)
        {
            if (_context.StrawberryFeedback == null)
            {
                return NotFound();
            }
            var strawberryFeedback = await _context.StrawberryFeedback.FindAsync(id);
            if (strawberryFeedback == null)
            {
                return NotFound();
            }

            _context.StrawberryFeedback.Remove(strawberryFeedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StrawberryFeedbackExists(int id)
        {
            return (_context.StrawberryFeedback?.Any(e => e.FeedbackId == id)).GetValueOrDefault();
        }
    }
}
