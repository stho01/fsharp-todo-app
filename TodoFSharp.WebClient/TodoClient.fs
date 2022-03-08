module TodoFSharp.WebClient.TodoClient

open System
open System.Net.Http
open System.Text
open TodoFSharp.WebClient.Dto
open TodoFSharp.WebClient.HttpMethods

let private client =
    let client = new HttpClient()
    let uri = "https://localhost:7157" |> Uri
    client.BaseAddress <- uri
    client

let internal getAllTodoListsRequest () = GET client "/"
let internal getTodoListRequest name = GET client $"/list/{name}"
let internal addTodoToListRequest name = POSTJson client $"/list/{name}/todo"
let internal removeTodoFromListRequest name id = DELETE client $"/list/{name}/todo/{id}"
let internal updateTodoUrl name = PUTJson client $"/list/{name}/todo"
let internal completeTodoRequest name id = POST client $"/list/{name}/todo/{id}/complete"
let internal uncompleteTodoRequest name id = POST client $"/list/{name}/todo/{id}/uncomplete"


let getTodoLists () =
    async {
        let! response = getAllTodoListsRequest ()
        
        return Utils.deserialize<PagedDataDto<TodoListDto>> response 
    }
    
let getTodoList name =
    async {
        let! response = getTodoListRequest name
        
        let result = Utils.deserialize<TodoListDto> response 
        
        return result
    }
    
let addTodoToList (name: string) (todo: TodoDto) =
    async {
        let! response = addTodoToListRequest name todo
        
        return Utils.deserialize<TodoListDto> response
    }
    
let removeTodoFromList (name: string) (id: Guid) =
    async {
        let! response = removeTodoFromListRequest name id
        
        return Utils.deserialize<TodoListDto> response
    }
    
let updateTodo (name: string) (todo: TodoDto) =
    async {
        let! response = updateTodoUrl name todo
            
        return Utils.deserialize<TodoListDto> response
    }
    
let completeTodo (listName: string) (id: string) =
    async {
        let! response = completeTodoRequest listName id
        
        return Utils.deserialize<TodoListDto> response
    }
    
let uncompleteTodo (listName: string) (id: string) =
    async {
        let! response = uncompleteTodoRequest listName id
        
        return Utils.deserialize<TodoListDto> response
    }