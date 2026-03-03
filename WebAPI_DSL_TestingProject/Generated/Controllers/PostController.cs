using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ExampleAPI.Generated.Entities;
using ExampleAPI.Generated.DbContext;
using ExampleAPI.Generated.Dtos;

namespace ExampleAPI.Generated.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly AppDbContext _context;

    public PostController(AppDbContext context)
    {
        _context = context;
    }
    

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost(PostDto dto)
    {
        var entity = dto.Adapt<Post>();
        _context.Posts.Add(entity);
        await _context.SaveChangesAsync();
        
        var resultDto = entity.Adapt<PostDto>();
        return CreatedAtAction("GetPost", new { id = entity.Id }, resultDto);

    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
    {
        var dtos = await _context.Posts.ProjectToType<PostDto>().ToListAsync();
        return Ok(dtos);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(Guid id)
    {
        var item = await _context.Posts
            .Where(x => x.Id == id)
            .ProjectToType<PostDto>()
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, PostDto dto)
    {
        var existingEntity = await _context.Posts.FindAsync(id);
        if (existingEntity == null) return NotFound();

        dto.Adapt(existingEntity); 
        existingEntity.Id = id;
        
        try 
        {
            await _context.SaveChangesAsync();
        } 
        catch (DbUpdateConcurrencyException) 
        {
            return Conflict();
        }
        return NoContent();

    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var item = await _context.Posts.FindAsync(id);
        if (item == null) return NotFound();
        
        _context.Posts.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}