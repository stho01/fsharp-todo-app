namespace TodoFSharp.Domain

open System

 // DOMAIN Models ===============================================
 
type TodoId = private TodoId of Guid
type TodoListName = private TodoListName of string

type Todo = {
    Id: TodoId
    Todo: string
    Done: bool
    CreatedDate: DateTimeOffset
}

type TodoList = {
    Name: TodoListName
    Todos: Todo list
}

 // DOMAIN Workflows ===============================================

type DomainError =
    | TodoListDoesNotExist of string
    | TodoDoesNotExist of string
    | GenericError of string
    | FailedToUpdateTodo of string
    | FailedToCreateList of string

type CreateList = string -> Result<TodoList, DomainError>

type AddTodoToList =
    TodoList
     -> Todo
     -> TodoList

type RemoveTodoFromList =
    TodoList
     -> Todo
     -> TodoList
     
type GetList = TodoListName -> TodoList option
type GetLists = unit -> TodoList list
    
type GetTodo =
    TodoListName
     -> TodoId
     -> TodoList option * Todo option 

type UpdateTodo = Todo -> Result<TodoList, DomainError>


// ===============================================

module TodoId =
    let newId () = Guid.NewGuid() |> TodoId
    let empty = Guid.Empty |> TodoId
    
    let create id =
        match box id with
        | :? Guid as id -> id |> TodoId |> Ok
        | :? string as s -> Guid.Parse s |> TodoId |> Ok
        | _ ->  Error "The TodoId must be a guid or a guid formated string"
    
    let createOption id =
        match box id with
        | :? Guid as id ->
            id |> TodoId  |> Some
        | :? string as s when String.IsNullOrWhiteSpace s = false ->
            Guid.Parse s |> TodoId |> Some
        | _ -> None
    
    let createOrGenerate id =
        match id with
        | Some id -> create id
        | None -> newId() |> Ok
    
    let value (TodoId id) = id
    
    let toString id = (value id).ToString()
    
module TodoListName =
    let noName = TodoListName "-"
    
    let create (name: string) =
        if name.Length < 40 then
            TodoListName name |> Ok
        else
            Error "Name cannot be longer than 40 characters"
            
    let createOption (name: string) =
        if String.IsNullOrWhiteSpace name then
            None
        elif name.Length < 40 then
            TodoListName name |> Some
        else
            None
    
    let value (TodoListName name) = name
    
    
module Todo =
     
    let create todo id =
        let id = TodoId.createOrGenerate id
        match id with
        | Ok id -> 
            { Id = id
              Todo = todo
              Done = false
              CreatedDate = DateTimeOffset.Now }
            |> Ok
        | Error err -> Error err
    
    let withId id = fun todo -> todo.Id = id
    
    let createdDate todo = todo.CreatedDate
        
        
module TodoList =
    let create name = { Name = name; Todos = [] }