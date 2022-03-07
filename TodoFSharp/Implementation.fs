module TodoFSharp.Implementation

open TodoFSharp.Domain

let internal allTodoExcept todo =
    fun item ->
        if item.Id <> todo.Id
        then Some item
        else None

let createTodoList
    checkExistence
    saveTodoList
    : CreateList =
        
    let todoList listName =
        match checkExistence listName with
        | (true, list) -> list
        | (false, _) ->
            listName
            |> TodoList.create
            |> saveTodoList
        
    fun name ->
        let validatedName = TodoListName.create name
        match TodoListName.create name with
        | Ok listName -> todoList listName |> Ok
        | Error err -> DomainError.GenericError err |> Error

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
        

let getTodoList
    fetchTodoList
    : GetList =
    fun name -> fetchTodoList name
    
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
    
    fun todo ->
        todoListName
        |> TodoListName.create
        |> Result.bind fetchTodoList
        |> Result.bind (updateTodo todo)
        |> Result.map saveTodoList
        |> Result.mapError DomainError.FailedToUpdateTodo