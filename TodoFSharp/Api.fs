module TodoFSharp.Api

open System
open System.IO
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.FSharp.Core
open TodoFSharp.Domain
open TodoFSharp.Dto

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

let private fetchTodoList (name: TodoListName): Result<TodoList, string> =
    name
    |> TodoListName.value
    |> sprintf "data/%s.json"
    |> Utils.readAllText 
    |> Utils.deserialize<TodoListDto>
    |> Result.map TodoListDto.toDomain
    
let private fetchTodoLists page take =
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
        |> List.map Utils.readAllText
        |> List.choose Utils.deserializeOption<TodoListDto>
    
    { Page = page 
      Total = files.Length
      Payload = payload }
    
        
// API ===============================================================


let getTodoListsRequestHandler =
    Func<Nullable<int>, Nullable<int>, IResult>(fun (page: Nullable<int>) (take: Nullable<int>) ->
        match (page.HasValue, take.HasValue) with
        | (true, true) -> Results.Ok (fetchTodoLists page.Value (Some take.Value))
        | (true, false) -> Results.Ok (fetchTodoLists page.Value None)
        | _ -> Results.Ok (fetchTodoLists 0 None) )
    
    
let getTodoListRequestHandler =
    let workflow =
        Implementation.getTodoList fetchTodoList

    let validate name =
        Ok name
        |> Result.bind TodoListName.create
        |> Result.bind workflow 
    
    Func<string, IResult>(
        fun name ->
            match validate name with
            | Ok todoList -> Results.Ok (todoList |> TodoListDto.toDto) 
            | Error error -> Results.BadRequest error)

let newTodoListRequestHandler =
    let workflow =
        Implementation.createTodoList
            listExists
            saveTodoList

    Func<string, IResult>(
        fun name ->
            match workflow name with
            | Ok todoList -> Results.Ok (todoList |> TodoListDto.toDto)
            | Error error -> Results.BadRequest error)

let addTodoToListRequestHandler =
    let addTodoToList = Implementation.addTodoToList saveTodoList
    let getTodoList = Implementation.getTodoList fetchTodoList
    
    let validateListName (name, todo) =
        match (TodoListName.create name) with
        | Ok name -> Ok (name, todo)
        | Error err -> Error err
    
    let validateTodo (name, todo) =
        Todo.create todo.Todo
        |> Result.map (fun todo -> (name, todo))
    
    let validateToDoList (name, todo) =
        match getTodoList name  with
        | Ok list -> (list, todo) |> Ok
        | Error err -> Error err
    
    let validate (name, todo) =
        Ok (name, todo)
        |> Result.bind validateListName
        |> Result.bind validateTodo
        |> Result.bind validateToDoList
    
    Func<string, TodoDto, IResult>(
        fun name todo ->
            match validate (name, todo) with
            | Ok (list, todo) ->
                let updateList =
                    addTodoToList list todo
                    |> TodoListDto.toDto
                    
                Results.Ok(updateList)
            | Error err -> Results.BadRequest err)

let removeTodoFromListRequestHandler =
    let workflow = Implementation.removeTodoFromList saveTodoList
    let getTodo = Implementation.getTodo fetchTodoList
    let validateName (name, id) =
        TodoListName.create name |> Result.map (fun name -> (name, id))
    let validateId (name, id) =
        TodoId.create id |> Result.map (fun id -> (name, id))
    let validateTodoExistence (name, id) =
        getTodo name id
        
    let validate (name, id) =
        Ok (name, id)
        |> Result.bind validateName
        |> Result.bind validateId
        |> Result.bind validateTodoExistence
    
    Func<string, Guid, IResult>(
        fun name id ->
            match validate (name, id) with
            | Ok (list, todo) ->
                let updatedList =
                    (workflow list todo)
                    |> TodoListDto.toDto
                    
                Results.Ok updatedList  
            | Error err -> Results.BadRequest err)

let updateTodoRequestHandler =
    Func<string, TodoDto, IResult>(
        fun name todo ->
            let workflow =
                Implementation.updateTodo
                    fetchTodoList
                    saveTodoList
                    name
            
            let domain = TodoDto.toDomain todo
            
            match workflow domain with
            | Ok result -> Results.Ok (result |> TodoListDto.toDto)
            | Error err -> Results.BadRequest err)