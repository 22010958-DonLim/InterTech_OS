using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using StrawberryHub.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace StrawberryHub.Controllers
{
    [Route("api/Comments")]
    [ApiController]
    public class StrawberryCommentsAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDbService _dbSvc;

        public StrawberryCommentsAPIController(AppDbContext context, IDbService dbsvc)
        {
            _context = context;
            _dbSvc = dbsvc;
        }

        // GET: api/StrawberryCommentsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryComment>>> GetStrawberryComment()
        {
          if (_context.StrawberryComment == null)
          {
              return NotFound();
          }
            return await _context.StrawberryComment.ToListAsync();
        }

        // GET: api/StrawberryCommentsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryComment>> GetStrawberryComment(int id)
        {
          if (_context.StrawberryComment == null)
          {
              return NotFound();
          }
            var strawberryComment = await _context.StrawberryComment.FindAsync(id);

            if (strawberryComment == null)
            {
                return NotFound();
            }

            return strawberryComment;
        }

        // PUT: api/StrawberryCommentsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStrawberryComment(int id, StrawberryComment strawberryComment)
        {
            if (id != strawberryComment.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(strawberryComment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StrawberryCommentExists(id))
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

        // POST: api/StrawberryCommentsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create/{userId}/{articleId}/{commentText}/{username}")]
        public async Task<ActionResult<StrawberryComment>> PostStrawberryComment(int userId, string commentText, int articleId, string username)
        {
         

            if (_context.StrawberryComment == null)
            {
                return Problem("Entity set 'AppDbContext.StrawberryComment'  is null.");
            }
            
            var matchUser = _context.StrawberryUser
                .Where(u => u.Username == username)
                .FirstOrDefault();

            if (matchUser == null)
            {
                return BadRequest("User does not exist");
            }

            int userIdd = userId;

            var strawberryComment = new StrawberryComment();
            strawberryComment.CommentDateTime = DateTime.Now;
            strawberryComment.UserId = userId;
            strawberryComment.ArticleId = articleId;
            strawberryComment.CommentText = commentText;

            _context.StrawberryComment.Add(strawberryComment);
            await _context.SaveChangesAsync();
            return Ok("Comment successfully on Article");

        }

        // DELETE: api/StrawberryCommentsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrawberryComment(int id)
        {
            if (_context.StrawberryComment == null)
            {
                return NotFound();
            }
            var strawberryComment = await _context.StrawberryComment.FindAsync(id);
            if (strawberryComment == null)
            {
                return NotFound();
            }

            _context.StrawberryComment.Remove(strawberryComment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted on Article"); ;
        }

        [HttpGet("TotalCommentsCount/{articleId}")]
        public async Task<ActionResult<StrawberryComment>> TotalComments(int articleId)
        {
            var articleComments = await _context.StrawberryComment
                .Include(c => c.StrawberryUser)
                .Where(c => c.ArticleId == articleId)
                .Select(c => new
                {
                    CommentId = c.CommentId,
                    UserId = c.UserId,
                    Username = c.StrawberryUser.Username,
                    CommentText = c.CommentText != "0" ? c.CommentText : "",
                    CommentDateTime = c.CommentText != "0" ? c.CommentDateTime : null
                })
                .ToListAsync();

            if (articleComments == null || articleComments.Count == 0)
            {
                return Ok(0);
            }


            return Ok(articleComments.Count());
        }

        [HttpGet("TotalComments/{articleId}")]
        public async Task<ActionResult<StrawberryComment>> GetArticleComments(int articleId)
        {
            
            var articleComments = await _context.StrawberryComment
                .Include(c => c.StrawberryUser)
                .Where(c => c.ArticleId == articleId)
                .Select(c => new
                {
                    CommentId = c.CommentId,
                    UserId = c.UserId,
                    Username = c.StrawberryUser.Username,
                    CommentText = c.CommentText != "0" ? c.CommentText : "",
                    CommentDateTime = c.CommentText != "0" ? c.CommentDateTime : null
                })
                .ToListAsync();

            if (articleComments == null || articleComments.Count == 0)
            {
                return Ok("There are no comments for this article yet");
            }



            return Ok(articleComments);
        }

        private bool StrawberryCommentExists(int id)
        {
            return (_context.StrawberryComment?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
