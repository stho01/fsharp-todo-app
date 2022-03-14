module TodoFSharp.Implementation

open Microsoft.Extensions.Logging
open TodoFSharp
open TodoFSharp.Domain

let internal allTodoExcept todo =
    fun item ->
        if item.Id <> todo.Id
        then Some item
        else None

let getTodoList
    fetchTodoList
    : GetList =
    fun name -> fetchTodoList name

let createTodoList
    fetchTodoList
    saveTodoList
    : CreateList =
        
    let doCreateList listName =
        match getTodoList fetchTodoList listName with
        | Some list -> list
        | None ->
            listName
            |> TodoList.create
            |> saveTodoList
        
    fun name ->
        
        match TodoListName.create name with
        | Ok listName -> doCreateList listName |> Ok
        | Error err -> DomainError.FailedToCreateList err |> Error

let addTodoToList
    saveTodoList
    : AddTodoToList =
        
    fun todoList todo -> 
        let updatedTodos = todo :: todoList.Todos
        { todoList with Todos = updatedTodos }
        |> saveTodoList
        
let removeTodoFromList
    saveTodoList
    : RemoveTodoFromList =
        
    fun todoList todo ->
        let updatedList =
            todoList.Todos
            |> List.choose (allTodoExcept todo)
        
        { todoList with Todos = updatedList }
        |> saveTodoList
        
    
let getTodo
    fetchTodoList
    : GetTodo =

    fun listName id ->
        match fetchTodoList listName with
        | Some list -> (Some list, list.Todos |> List.tryFind (Todo.withId id))
        | None -> (None, None)

let getTodoLists fetchTodoLists : GetLists =
    fun () -> fetchTodoLists ()
    
let updateTodo
    fetchTodoList
    saveTodoList
    todoListName
    : UpdateTodo =
    
    let append todo list =
        let updated = todo :: list
        updated
    
    let doesNotExist todo list =
        let result =
            list.Todos
            |> List.exists (fun a -> a.Id = todo.Id)
        
        if result = true then
            Some todo
        else
            None
            
    let doUpdate list todo =
        let updated =
            list.Todos
            |> List.choose (allTodoExcept todo)
            |> (append todo)
            
        Ok { list with Todos = updated }
    
    let updateTodo todo list =
        let todo = doesNotExist todo list
        match todo with
        | Some todo -> doUpdate list todo
        | None -> Error "Failed to update todo. Todo entry was not found..."
    
    let fetchTodoList name =
        match (fetchTodoList name) with
        | Some list -> Ok list
        | None -> Error "Failed to fetch todo list"
    
    fun todo ->
        todoListName
        |> TodoListName.create
        |> Result.bind fetchTodoList
        |> Result.bind (updateTodo todo)
        |> Result.map saveTodoList
        |> Result.mapError DomainError.FailedToUpdateTodo

let deleteTodolist
    (logger: ILogger)
    deleteTodoList
    : DeleteTodoList =
        
        
    fun name ->
        let name = TodoListName.value name
        match deleteTodoList name with
        | Ok _ -> logger.LogInformation $"To-do list {name} was deleted successfully" 
        | Error err -> logger.LogError err 
        ()