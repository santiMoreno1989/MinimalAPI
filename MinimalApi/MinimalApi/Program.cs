using Microsoft.EntityFrameworkCore;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region TODO_CRUD

app.MapPost("/todoItems", async (Todo todo, TodoDbContext dbContext)
    =>{
    dbContext.Todos.Add(todo);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/todoItems{todo.Id}", todo);
});

app.MapGet("/todoItems", async (TodoDbContext dbContext)
    => await dbContext.Todos.ToListAsync());

app.MapGet("/todoItems/complete", async (TodoDbContext dbContext)
    => await dbContext.Todos.Where(t=> t.IsComplete==true).ToListAsync());

app.MapGet("/todoItems/{id}", async (int id, TodoDbContext dbContext) 
    => await dbContext.Todos.FindAsync(id)
        is Todo todo ? Results.Ok(todo) : Results.NotFound());

app.MapPut("/todoImtes/{id}", async(int id,Todo inputTodo, TodoDbContext dbContext) 
    => {
        var todo = await dbContext.Todos.FindAsync(id);
        
        if (todo is null) return Results.NotFound();
        
        todo.Name=inputTodo.Name;
        todo.IsComplete=inputTodo.IsComplete;
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    });

app.MapDelete("/todoItems/{id}", async (int id,TodoDbContext dbContext)
    => {
        if (await dbContext.Todos.FindAsync(id) is Todo todo) 
        {
            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync();
            return Results.Ok(todo);
        }
        return Results.NotFound();  
    });
#endregion

app.Run();
