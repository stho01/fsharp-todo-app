module TodoFSharp.WebClient.TodoClient

open System
open System.Net.Http
open System.Text
open System.Text.Encodings.Web
open Microsoft.VisualBasic.CompilerServices
open TodoFSharp.WebClient.Dto

let baseUrl = "https://localhost:7157"
let getAllTodoListsUrl = "/"
let getTodoListUrl name = $"/list/{name}"
let addTodoToListUrl name = $"/list/{name}/todo"
let removeTodoFromListUrl name id = $"/list/{name}/todo/{id}"

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
    
let addTodoToList (name: string) (todo: TodoDto) =
    async {
        let url = addTodoToListUrl name
        let json = Utils.serialize todo

        let requestBody = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");
        
        let! response =
            client.PostAsync(url, requestBody)
            |> Async.AwaitTask
            
        let! responseBody =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask
        
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