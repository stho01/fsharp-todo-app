module TodoFSharp.WebClient.TodoClient

open System
open System.Net.Http
open TodoFSharp.WebClient.Dto

let baseUrl = "https://localhost:7157"
let getAllTodoListsUrl = "/"
let getTodoListUrl name = $"/list/{name}"

let private client =
    let client = new HttpClient()
    let uri = baseUrl |> Uri
    client.BaseAddress <- uri
    client

let getTodoLists () =
    async {
        let! response = client.GetAsync(getAllTodoListsUrl) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        let result = Utils.deserialize<PagedDataDto<TodoListDto>> content 
        
        return result
    }
    
let getTodoList name =
    async {
        let url = getTodoListUrl name
        let! response = client.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        let result = Utils.deserialize<TodoListDto> content 
        
        return result
    }