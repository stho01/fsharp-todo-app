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
    : CreateTodoList =
        
    fun name ->
        match TodoListName.create name with
        | Ok listName ->
            match checkExistence listName with
            | true -> Error "Failed to create list, list with that name already exists."
            | false ->
                listName
                |> TodoList.create
                |> saveTodoList
                |> Ok
                
        | Error err -> Error err

let addTodoToList
    saveTodoList
    : AddTodoToTodoList =
        
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
    : GetTodoList =
    fun name ->
        let todoList = fetchTodoList name
    
        match todoList with
        | Ok todoList -> todoList |> Ok
        | Error err -> Error err

let getTodo
    fetchTodoList
    : GetTodo =
    fun listName id ->
        
        match fetchTodoList listName with
        | Ok list ->
            let todoOption = list.Todos |> List.tryFind (Todo.withId id)
            match todoOption with
            | Some todo -> (list, todo) |> Ok
            | None -> Error $"Todo with ID {id} was not found" 
        | Error err -> Error err

let getTodoLists fetchTodoLists : GetTodoLists =
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
            Ok todo
        else
            Error $"Todo with {todo.Id} does not exists."
            
    let doUpdate list todo =
        let updated =
            list.Todos
            |> List.choose (allTodoExcept todo)
            |> (append todo)
            
        Ok { list with Todos = updated }
    
    let updateTodo todo list =
        doesNotExist todo list
        |> Result.bind (doUpdate list)
        
    fun todo ->
         todoListName
         |> TodoListName.create
         |> Result.bind fetchTodoList
         |> Result.bind (updateTodo todo)
         |> Result.map saveTodoList