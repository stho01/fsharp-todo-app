module TodoFSharp.WebClient.TodoClient

open System
open System.Net.Http
open TodoFSharp.WebClient.Dto
open TodoFSharp.WebClient

let private client =
    let client = new HttpClient()
    let uri = "https://localhost:7157" |> Uri
    client.BaseAddress <- uri
    client
    
module Queries =
    let getTodoLists () =
        let GET = HttpMethods.createGetRequest client Json.strictDeserializer<PagedDataDto<TodoListDto>> 
        
        GET "/"

    let getTodoList name =
        let GET = HttpMethods.createGetRequest client Json.strictDeserializer<TodoListDto>
        
        GET $"/list/{name}"

module Commands =

    let createTodoList name =
        let POST = HttpMethods.createPostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}" None 
    
    let addTodoToList name todo =
        let POST = HttpMethods.createPostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}/todo" (Some todo)
        
    let removeTodoFromList name id =
        let DELETE = HttpMethods.createDeleteRequest client Json.strictDeserializer<TodoListDto>
        
        DELETE $"/list/{name}/todo/{id}"

    let updateTodo name todo =
        let PUT = HttpMethods.createPutRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        PUT $"/list/{name}/todo" (Some todo)

    let completeTodo name id =
        let POST = HttpMethods.createPostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}/todo/{id}/complete" None

    let incompleteTodo name id =
        let POST = HttpMethods.createPostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}/todo/{id}/incomplete" None