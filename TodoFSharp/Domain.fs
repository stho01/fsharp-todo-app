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

type CreateList =
    string
     -> Result<TodoList, string>

type AddTodoToList =
    TodoList
     -> Todo
     -> TodoList
     
type RemoveTodoFromList =
    TodoList
     -> Todo
     -> TodoList
     
type GetList =
    TodoListName -> Result<TodoList, string>
    
type GetLists =
    unit -> Result<TodoList list, string>
    
type GetTodo =
    TodoListName
     -> TodoId
     -> Result<TodoList * Todo, string>

type UpdateTodo = Todo -> Result<TodoList, string>


// ===============================================

module TodoId =
    let newId () = Guid.NewGuid() |> TodoId
    let empty = Guid.Empty |> TodoId
    
    let create id =
        match box id with
        | :? Guid as id -> id |> TodoId |> Ok
        | :? string as s -> Guid.Parse s |> TodoId |> Ok
        | _ -> Error "The TodoId must be a guid"
        
    let createOption id =
        match box id with
        | :? Guid as id ->
            id |> TodoId  |> Some
        | :? string as s when String.IsNullOrWhiteSpace s = false ->
            Guid.Parse s |> TodoId |> Some
        | _ -> None
        
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
    let create todo =
        { Id = TodoId.newId()
          Todo = todo
          Done = false
          CreatedDate = DateTimeOffset.Now }
        |> Ok
    
    let withId id = fun todo -> todo.Id = id
        
        
module TodoList =
    let create name = { Name = name; Todos = [] }