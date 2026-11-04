using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Todos.DTOs;
using SmartMealPlanner.Application.Todos.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly IValidator<CreateTodoRequest> _createValidator;
    private readonly IValidator<UpdateTodoRequest> _updateValidator;
    private readonly IValidator<TodoListQuery> _listQueryValidator;
    private readonly IValidator<MarkTodoCompleteRequest> _markCompleteValidator;

    public TodosController(
        ITodoService todoService,
        IValidator<CreateTodoRequest> createValidator,
        IValidator<UpdateTodoRequest> updateValidator,
        IValidator<TodoListQuery> listQueryValidator,
        IValidator<MarkTodoCompleteRequest> markCompleteValidator)
    {
        _todoService = todoService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _listQueryValidator = listQueryValidator;
        _markCompleteValidator = markCompleteValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PaginatedList<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<PaginatedList<TodoDto>>>> GetTodos(
        [FromQuery] TodoListQuery query,
        CancellationToken cancellationToken)
    {
        await _listQueryValidator.ValidateAndThrowAsync(query, cancellationToken);
        var todos = await _todoService.GetTodosAsync(query, cancellationToken);
        return Ok(ApiResult<PaginatedList<TodoDto>>.Ok(todos));
    }

    [HttpGet("today")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<TodoDto>>>> GetTodayTasks(
        CancellationToken cancellationToken)
    {
        var todos = await _todoService.GetTodayTasksAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<TodoDto>>.Ok(todos));
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<TodoDto>>>> GetUpcomingTasks(
        CancellationToken cancellationToken)
    {
        var todos = await _todoService.GetUpcomingTasksAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<TodoDto>>.Ok(todos));
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<TodoDto>>>> GetOverdueTasks(
        CancellationToken cancellationToken)
    {
        var todos = await _todoService.GetOverdueTasksAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<TodoDto>>.Ok(todos));
    }

    [HttpGet("recurring")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<TodoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<TodoDto>>>> GetRecurringTasks(
        CancellationToken cancellationToken)
    {
        var todos = await _todoService.GetRecurringTasksAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<TodoDto>>.Ok(todos));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<TodoDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var todo = await _todoService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResult<TodoDto>.Ok(todo));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<TodoDto>>> Create(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var todo = await _todoService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, ApiResult<TodoDto>.Ok(todo));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<TodoDto>>> Update(
        Guid id,
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var todo = await _todoService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResult<TodoDto>.Ok(todo));
    }

    [HttpPatch("{id:guid}/complete")]
    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<TodoDto>>> MarkComplete(
        Guid id,
        [FromBody] MarkTodoCompleteRequest request,
        CancellationToken cancellationToken)
    {
        await _markCompleteValidator.ValidateAndThrowAsync(request, cancellationToken);
        var todo = await _todoService.MarkCompleteAsync(id, request, cancellationToken);
        return Ok(ApiResult<TodoDto>.Ok(todo));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _todoService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
