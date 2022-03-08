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

let internal getAllTodoListsUrl = "/"
let internal getTodoListRequest name = $"/list/{name}"
let internal addTodoToListUrl name = $"/list/{name}/todo"
let internal removeTodoFromListUrl name id = $"/list/{name}/todo/{id}"
let internal updateTodoUrl name = $"/list/{name}/todo"
let internal completeTodoUrl name id = $"/list/{name}/todo/{id}/complete"
let internal uncompleteTodoUrl name id = $"/list/{name}/todo/{id}/uncomplete"


let getTodoLists () =
    async {
        let! response = client.GetAsync(getAllTodoListsUrl) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        let result = Utils.deserialize<PagedDataDto<TodoListDto>> content 
        
        return result
    }
    
let getTodoList name =
    async {
        let url = getTodoListRequest name
        let! response = client.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        let result = Utils.deserialize<TodoListDto> content 
        
        return result
    }
    
let addTodoToList (name: string) (todo: TodoDto) =
    async {
        let url = addTodoToListUrl name
        let! responseBody = POSTJson client url todo
        return Utils.deserialize<TodoListDto> responseBody
    }
    
let removeTodoFromList (name: string) (id: Guid) =
    async {
        let url = removeTodoFromListUrl name id
        
        let! response =
            client.DeleteAsync(url)
            |> Async.AwaitTask
            
        let! responseBody =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask
        
        return Utils.deserialize<TodoListDto> responseBody
    }
    
let updateTodo (name: string) (todo: TodoDto) =
    async {
        let url = updateTodoUrl name
        let json = Utils.serialize todo
        
        let requestBody = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");
        
        let! response = client.PutAsync(url, requestBody) |> Async.AwaitTask
        
        let! responseBody =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask
            
        return Utils.deserialize<TodoListDto> responseBody
    }
    
let completeTodo (listName: string) (id: string) =
    async {
        let url = completeTodoUrl listName id
        let! result = POST client url
        return Utils.deserialize<TodoListDto> result
    }
    
let uncompleteTodo (listName: string) (id: string) =
    async {
        let url = uncompleteTodoUrl listName id
        let! result = POST client url
        return Utils.deserialize<TodoListDto> result
    }