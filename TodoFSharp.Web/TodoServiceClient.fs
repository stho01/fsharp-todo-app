module TodoFSharp.Web.TodoServiceClient

open System
open System.Net.Http

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
        return content
    }
    
let getTodoList name =
    async {
        let url = getTodoListUrl name
        let! response = client.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        return content
    }