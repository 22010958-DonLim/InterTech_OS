using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;
using StrawberryHub.Services;

namespace StrawberryHub.Controllers
{
    [Route("api/likecomment")]
    [ApiController]
    public class LikeCommentsAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LikeCommentsAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LikeCommentsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrawberryLikeComment>>> GetStrawberryLikeComment()
        {
          if (_context.StrawberryLikeComment == null)
          {
              return NotFound();
          }
            return await _context.StrawberryLikeComment.ToListAsync();
        }

        // GET: api/LikeCommentsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StrawberryLikeComment>> GetStrawberryLikeComment(int id)
        {
          if (_context.StrawberryLikeComment == null)
          {
              return NotFound();
          }
            var strawberryLikeComment = await _context.StrawberryLikeComment.FindAsync(id);

            if (strawberryLikeComment == null)
            {
                return NotFound();
            }

            return strawberryLikeComment;
        }

        // POST: api/LikeCommentAPI
        [HttpPost("Create/{userId}/{commentText}/{likes}/{articleId}")]
        public async Task<IActionResult> CreateLikeComment(int userId, string commentText, int likes, int articleId)
        {
            try
            {
                // Your logic to create a new like/comment here
                // Example: Save the like/comment to the database
                // Set LikeTimestamp if Likes is 1
                var newLikeComment = new StrawberryLikeComment();
                if (likes == 1)
                {
                    newLikeComment.LikeTimestamp = DateTime.Now;
                }
                else
                {
                    newLikeComment.LikeTimestamp = null;
                }

                // Set CommentTimestamp if CommentText is not null or empty
                if (!string.IsNullOrEmpty(commentText))
                {
                    newLikeComment.CommentTimestamp = DateTime.Now;
                }
                else
                {
                    newLikeComment.CommentTimestamp = null;
                }

                newLikeComment.UserId = userId;
                newLikeComment.ArticleId = articleId;
                newLikeComment.Likes = likes;
                newLikeComment.CommentText = commentText; 

                _context.StrawberryLikeComment.Add(newLikeComment);
                await _context.SaveChangesAsync();

                return Ok("Like/Comment created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // PUT: api/LikeCommentsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStrawberryLikeComment(int id, StrawberryLikeComment strawberryLikeComment)
        {
            if (id != strawberryLikeComment.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(strawberryLikeComment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StrawberryLikeCommentExists(id))
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

        // POST: api/LikeCommentsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StrawberryLikeComment>> PostStrawberryLikeComment(StrawberryLikeComment strawberryLikeComment)
        {
          if (_context.StrawberryLikeComment == null)
          {
              return Problem("Entity set 'AppDbContext.StrawberryLikeComment'  is null.");
          }
            _context.StrawberryLikeComment.Add(strawberryLikeComment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StrawberryLikeCommentExists(strawberryLikeComment.CommentId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStrawberryLikeComment", new { id = strawberryLikeComment.CommentId }, strawberryLikeComment);
        }

        // DELETE: api/LikeCommentsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrawberryLikeComment(int id)
        {
            if (_context.StrawberryLikeComment == null)
            {
                return NotFound();
            }
            var strawberryLikeComment = await _context.StrawberryLikeComment.FindAsync(id);
            if (strawberryLikeComment == null)
            {
                return NotFound();
            }

            _context.StrawberryLikeComment.Remove(strawberryLikeComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/ArticleCommentsAPI/{articleId}
        [HttpGet("total/{articleId}")]
        public async Task<ActionResult<StrawberryLikeComment>> GetArticleComments(int articleId)
        {
            var articleComments = await _context.StrawberryLikeComment
                .Where(c => c.ArticleId == articleId)
                .Select(c => new
                {
                    CommentId = c.CommentId,
                    UserId = c.UserId,
                    CommentText = c.CommentText,
                    CommentTimestamp = c.CommentTimestamp,
                    Likes = c.Likes,
                    LikeTimestamp = c.LikeTimestamp
                })
                .ToListAsync();

            if (articleComments == null || articleComments.Count == 0)
            {
                return NotFound();
            }

            return Ok(articleComments);
        }

        private bool StrawberryLikeCommentExists(int id)
        {
            return (_context.StrawberryLikeComment?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
