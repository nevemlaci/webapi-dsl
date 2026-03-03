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
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }
    

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(UserDto dto)
    {
        var entity = dto.Adapt<User>();
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        
        var resultDto = entity.Adapt<UserDto>();
        return CreatedAtAction("GetUser", new { id = entity.Id }, resultDto);

    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var dtos = await _context.Users.ProjectToType<UserDto>().ToListAsync();
        return Ok(dtos);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var item = await _context.Users
            .Where(x => x.Id == id)
            .ProjectToType<UserDto>()
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return Ok(item);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UserDto dto)
    {
        var existingEntity = await _context.Users.FindAsync(id);
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
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var item = await _context.Users.FindAsync(id);
        if (item == null) return NotFound();
        
        _context.Users.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}