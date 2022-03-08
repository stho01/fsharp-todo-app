module TodoFSharp.Api

open System
open System.IO
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Core
open TodoFSharp.Domain
open TodoFSharp.Dto
open TodoFSharp.WebClient.Dto

// Services ====================================================

let private resolveUrl todoListName = $"/list/{todoListName}"

let private listExists listName =
    let file = $"data/{listName}.json" |> FileInfo
    file.Exists

let private saveTodoList (list: TodoList) =
    let listName = TodoListName.value list.Name
    
    list
    |> TodoListDto.toDto
    |> Utils.serialize
    |> Utils.writeAllText $"data/{listName}.json"
    
    list

let private fetchTodoList
    (logger: ILogger)
    (name: TodoListName) =
        
        name
        |> TodoListName.value
        |> sprintf "data/%s.json"
        |> (Utils.readAllTextOption logger) 
        |> Option.bind Utils.deserializeOption<TodoListDto>
        |> Option.map TodoListDto.toDomain
    
let private fetchTodoLists logger page take =
    let files =
        Directory.GetFiles "data"
        |> Array.choose Utils.jsonFile

    let take =
        match take with
        | Some take -> take
        | None -> 5
        |> min files.Length
        |> max 1
        
    let page =
        page
        |> min (files.Length / take)
        |> max 1
        
    let skip =
        take * page
        |> min (files.Length - take)
        |> max 0
    
    let payload =
        files
        |> Array.toList
        |> List.skip skip
        |> List.take take
        |> List.map Utils.fileName
        |> List.map (Utils.readAllTextOption logger)
        |> List.choose (Option.bind Utils.deserializeOption<TodoListDto>)
    
    { Page = page 
      Total = files.Length
      Payload = payload }
    
        
// API ===============================================================


let getTodoListsRequestHandler =
    Func<ILoggerFactory, Nullable<int>, Nullable<int>, IResult>(
        fun ([<FromServices>] loggerFactory: ILoggerFactory) (page: Nullable<int>) (take: Nullable<int>) ->
            let logger = loggerFactory.CreateLogger "GetTodoListRequest"
            let page = page.GetValueOrDefault 0
            let take = take.GetValueOrDefault 5 |> Some
            
            Results.Ok (fetchTodoLists logger page take)
        )
    
let getTodoListRequestHandler =
    Func<ILoggerFactory, string, IResult>(
        fun loggerFactory name ->
            let logger = loggerFactory.CreateLogger "getTodoListRequestHandler"
            let workflow =
                Implementation.getTodoList (fetchTodoList logger)

            let validate name =
                Ok name
                |> Result.bind TodoListName.create
            
            match validate name with
            | Ok listName ->
                match workflow listName with
                | Some todoList -> Results.Ok (todoList |> TodoListDto.toDto)
                | None -> Results.NotFound $"List with name {TodoListName.value listName} was not found"
            | Error error -> Results.BadRequest error)

let newTodoListRequestHandler =
    Func<ILoggerFactory, string, IResult>(
        fun loggerFactory name ->
            let logger = loggerFactory.CreateLogger "newTodoListRequestHandler"
    
            let workflow =
                Implementation.createTodoList
                    (fetchTodoList logger) 
                    saveTodoList

            match workflow name with
            | Ok todoList -> Results.Ok (todoList |> TodoListDto.toDto)
            | Error error -> Results.BadRequest error)

let addTodoToListRequestHandler =
    let validateListName (name, todo) = (TodoListName.create name) |> Result.map (fun name -> (name, todo))
    let validateTodo (name, todo) = Todo.create todo.Todo |> Result.map (fun todo -> (name, todo))

    let validate (name, todo) =
        Ok (name, todo)
        |> Result.bind validateListName
        |> Result.bind validateTodo
    
    Func<ILoggerFactory, string, TodoDto, IResult>(
        fun loggerFactory name todo ->
            let logger = loggerFactory.CreateLogger "addTodoToListRequestHandler"
            let addTodoToList = Implementation.addTodoToList saveTodoList
            let getTodoList = Implementation.getTodoList (fetchTodoList logger)
            
            match validate (name, todo) with
            | Ok (name, todo) ->
                match getTodoList name with
                | Some list -> Results.Ok (addTodoToList list todo |> TodoListDto.toDto)
                | None -> Results.NotFound $"Todo list with name {TodoListName.value name} not found"
            | Error err -> Results.BadRequest err)

let removeTodoFromListRequestHandler =
    let validateName (name, id) =
        TodoListName.create name |> Result.map (fun name -> (name, id))
    let validateId (name, id) =
        TodoId.create id |> Result.map (fun id -> (name, id))

    let validate (name, id) =
        Ok (name, id)
        |> Result.bind validateName
        |> Result.bind validateId
    
    Func<ILoggerFactory, string, Guid, IResult>(
        fun loggerFactory name id ->
            let logger = loggerFactory.CreateLogger "removeTodoFromListRequestHandler"
            let removeTodoFromListWorkflow = Implementation.removeTodoFromList saveTodoList
            let getTodo = Implementation.getTodo (fetchTodoList logger)
            let getTodoList = Implementation.getTodoList (fetchTodoList logger)        
            
            match validate (name, id) with
            | Ok (name, id) ->
                
                match getTodo name id with
                | (list, todo) ->
                    match (list, todo) with
                    | (Some list, Some todo) ->
                        let result = removeTodoFromListWorkflow list todo |> TodoListDto.toDto
                        Results.Ok result
                    | (Some _, None) ->
                        Results.NotFound $"Todo with id {id} not found"
                    | _ ->
                        Results.NotFound $"TodoList with name {TodoListName.value name} not found"
                        
            | Error err -> Results.BadRequest err)

let updateTodoRequestHandler =
    Func<ILoggerFactory, string, TodoDto, IResult>(
        fun loggerFactory name todo ->
            let logger = loggerFactory.CreateLogger "updateTodoRequestHandler"
            let workflow =
                Implementation.updateTodo
                    (fetchTodoList logger)
                    saveTodoList
                    name
            
            let domain = TodoDto.toDomain todo
            
            match workflow domain with
            | Ok result -> Results.Ok (result |> TodoListDto.toDto)
            | Error err -> Results.BadRequest err)