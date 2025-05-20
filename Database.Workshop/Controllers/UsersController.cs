using System.Diagnostics;
using Database.Workshop.Data;
using Database.Workshop.Models;
using Database.Workshop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Database.Workshop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(RepositoryFactory factory, ILogger<UsersController> logger) : ControllerBase
{
    private readonly IUserRepository _repo = factory.Create();

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sw = Stopwatch.StartNew();
        var result = await _repo.GetAllAsync();
        sw.Stop();

        logger.LogInformation("GET /api/users took {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var sw = Stopwatch.StartNew();
        var result = await _repo.GetByIdAsync(id);
        sw.Stop();

        logger.LogInformation("GET /api/users/{Id} took {ElapsedMilliseconds}ms", id, sw.ElapsedMilliseconds);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add(User user)
    {
        var sw = Stopwatch.StartNew();
        await _repo.AddAsync(user);
        sw.Stop();

        logger.LogInformation("POST /api/users took {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(User user)
    {
        var sw = Stopwatch.StartNew();
        await _repo.UpdateAsync(user);
        sw.Stop();

        logger.LogInformation("PUT /api/users took {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sw = Stopwatch.StartNew();
        await _repo.DeleteAsync(id);
        sw.Stop();

        logger.LogInformation("DELETE /api/users/{Id} took {ElapsedMilliseconds}ms", id, sw.ElapsedMilliseconds);
        return Ok();
    }
    [HttpPost("benchmark/individual")]
    public async Task<IActionResult> BenchmarkIndividual()
    {
        var times = await _repo.BenchmarkIndividualAsync();
        return Ok(new
        {
            InsertTime = times.ElementAt(0).TotalMilliseconds,
            UpdateTime = times.ElementAt(1).TotalMilliseconds,
            DeleteTime = times.ElementAt(2).TotalMilliseconds
        });
    }

    [HttpPost("benchmark/batch")]
    public async Task<IActionResult> BenchmarkBatch()
    {
        var times = await _repo.BenchmarkBatchAsync();
        return Ok(new
        {
            InsertTime = times.ElementAt(0).TotalMilliseconds,
            UpdateTime = times.ElementAt(1).TotalMilliseconds,
            DeleteTime = times.ElementAt(2).TotalMilliseconds
        });
    }
}
