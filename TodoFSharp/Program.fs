open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.Extensions.Hosting
open TodoFSharp

let buildCorsPolicy =
    Action<CorsPolicyBuilder>(
        fun builder ->
            builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                |> ignore
            ())

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    
    let app = builder.Build()
    
    app.MapGet("/", Api.getTodoListsRequestHandler) |> ignore
    app.MapGet("/list/{name}", Api.getTodoListRequestHandler) |> ignore
    app.MapPost("/list/{name}", Api.newTodoListRequestHandler) |> ignore
    app.MapPost("/list/{name}/todo", Api.addTodoToListRequestHandler) |> ignore
    app.MapDelete("/list/{name}/todo/{id}", Api.removeTodoFromListRequestHandler) |> ignore
    app.MapPut("/list/{name}/todo", Api.updateTodoRequestHandler) |> ignore
    
    app.Run()

    0 // Exit code

