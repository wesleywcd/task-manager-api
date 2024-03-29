using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAPI.Data;
using TaskAPI.Data.Entities;
using TaskAPI.Views;

namespace TaskAPI.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly TaskContext _context;

    public TaskController(
        IMapper mapper,
        TaskContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    [HttpGet(Name = "GetTasks")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get([FromQuery] int offset, int limit, Priority? priority, Status? status)
    {
        IEnumerable<TaskModel> query = _context.Tasks;

        if (priority.HasValue)
            query = query.Where(x => x.Priority == priority);
        if (status.HasValue)
            query = query.Where(x => x.Status == status);
        
        var list = query
            .Skip(offset)
            .Take(limit)
            .ToList();

        return Ok(_mapper.Map<List<TaskView>>(list));
    }
    
    [HttpGet("chart", Name = "GetChart")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetChart()
    {
        var totalTasks = _context.Tasks.Count();
        
        var list = _context.Tasks
            .GroupBy(t => t.Priority)
            .Select(g => new { key = g.Key, value = ((double)g.Count() / totalTasks) * 100 })
            .ToList();
    
        return Ok(list);
    }

    [HttpGet("{id}", Name = "GetById")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var task = _context.Tasks.SingleOrDefault(x=> x.Id.Equals(id));
        if (task == null)
            return NotFound();
        return Ok(_mapper.Map<TaskView>(task));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Create(TaskView view)
    {
        var model = _mapper.Map<TaskModel>(view);
        if (model == null)
            return BadRequest();

        _context.Tasks.Add(model);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }
    
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Update(TaskView view)
    {
        var model = _context.Tasks.FirstOrDefault(x => x.Id == view.Id);
        if (model == null)
            return NotFound();

        model.Title = view.Title;
        model.DueDate = view.DueDate;
        model.Description = view.Title;
        model.Status = view.Status;
        model.Priority = view.Priority;

        _context.Tasks.Update(model);
        _context.SaveChanges();

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(int id)
    {
        var model = _context.Tasks.SingleOrDefault(d => d.Id == id);

        if (model == null)
            return NotFound();

        _context.Tasks.Remove(model);
        _context.SaveChanges();

        return NoContent();
    }
}