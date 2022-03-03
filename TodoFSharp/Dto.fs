namespace TodoFSharp.Dto

open System
open TodoFSharp.Domain

type TodoDto = {
    Id: string option
    Todo: string
    Done: bool option
    CreatedDate: DateTimeOffset option
}

type TodoListDto = {
    Name: string
    Todos: TodoDto list option
}

type TodoListDetailsDto = {
    Name: string
    NumberOfTodos: int
    Url: string
}

module TodoDto =
    
    let private getId dto =
        dto.Id
        |> Option.bind TodoId.createOption
    
    let toDomain dto: Todo =
        let id =
            match getId dto with
            | Some id -> id
            | None -> TodoId.empty
            
        let isDone =
            match dto.Done with
            | Some isDone -> isDone
            | None -> false
            
        let createdDate =
            match dto.CreatedDate with
            | Some createdDate -> createdDate
            | None -> DateTimeOffset.MinValue
            
        { Id = id
          Todo = dto.Todo
          Done = isDone
          CreatedDate = createdDate }
        
    let toDto (domain: Todo) =
        { Id = domain.Id |> TodoId.toString |> Some   
          Todo = domain.Todo
          Done = domain.Done |> Some
          CreatedDate = domain.CreatedDate |> Some }


module TodoListDto =
    let toDomain (dto: TodoListDto): TodoList =
        let name =
            match TodoListName.createOption dto.Name with
            | Some name -> name
            | None -> TodoListName.noName
        
        let todos =
            match dto.Todos with
            | Some todos ->
                todos
                |> List.map TodoDto.toDomain
            | None -> []
        
        { Name = name
          Todos = todos }
        
    let toDto (domain: TodoList) =
        { Name = domain.Name |> TodoListName.value
          Todos = domain.Todos |> List.map TodoDto.toDto |> Some }
        
    let numberOfTodos dto =
        match dto.Todos with
        | Some list -> list |> List.length
        | None -> 0
        
    let toDetails resolveUrl (dto: TodoListDto): TodoListDetailsDto =
        { Name = dto.Name
          NumberOfTodos = dto |> numberOfTodos
          Url = dto.Name |> resolveUrl }
        

module TodoListDetailsDto =
    let toDto toUrl (domain: TodoList) =
        { Name = domain.Name |> TodoListName.value
          NumberOfTodos = domain.Todos |> List.length
          Url = domain.Name |> toUrl  }
        
    