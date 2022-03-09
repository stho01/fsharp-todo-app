namespace TodoFSharp.WebClient.Dto

open System

[<CLIMutable>]
type TodoDto = {
    Id: Guid option
    Todo: string
    Done: bool option
    CreatedDate: DateTimeOffset option
}

[<CLIMutable>]
type TodoListDto = {
    Name: string
    Todos: TodoDto list option
}

[<CLIMutable>]
type TodoListDetailsDto = {
    Name: string
    NumberOfTodos: int
    Url: string
}

[<CLIMutable>]
type PagedDataDto<'TItem> = {
    Page: int
    Total: int
    Payload: 'TItem list 
}
