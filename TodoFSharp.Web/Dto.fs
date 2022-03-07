namespace TodoFSharp.Web.Dto

open System
open Microsoft.FSharp.Core
open TodoFSharp.Web.ViewModels
open TodoFSharp.WebClient.Dto
    
[<CLIMutable>]
type NewTodoDto = {
    Id: Guid
    Todo: string
}
    
module PagedResultDto =
    
    let toViewModel<'TItemDto, 'TItem> converter (dto: PagedDataDto<'TItemDto>): PagedData<'TItem> =
        let payload = dto.Payload |> List.map converter 
        
        { Page = dto.Page; Total = dto.Total; Payload = payload }
    
module TodoDto =
    let private idOption (dto: TodoDto) =
        try
            match dto.Id with
            | Some id -> id |> Some
            | None -> None
        with
        | _ -> None
    
    let toViewModelOption (dto: TodoDto): Todo option =
        let id = idOption dto
        match id with
        | None -> None
        | Some id ->
            let isDone =
                match dto.Done with
                | Some isDone -> isDone
                | None -> false
                
            let createdDate =
                match dto.CreatedDate with
                | Some createdDate -> createdDate
                | None -> DateTimeOffset.MinValue
                
            Some { Id = id; Todo = dto.Todo; Done = isDone; CreatedDate = createdDate }
        
module TodoListDto =
    let toViewModel (dto: TodoListDto): TodoList =
        let name = dto.Name
        let todos =
            match dto.Todos with
            | Some list -> list
            | None -> []
            |> List.choose TodoDto.toViewModelOption
        
        { Name = name; Todos = todos }