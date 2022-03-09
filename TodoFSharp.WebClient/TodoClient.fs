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
        let GET = HttpMethods.CreateGetRequest client Json.strictDeserializer<PagedDataDto<TodoListDto>> 
        
        GET "/"

    let getTodoList name =
        let GET = HttpMethods.CreateGetRequest client Json.strictDeserializer<TodoListDto>
        
        GET $"/list/{name}"

module Commands =

    let addTodoToList name todo =
        let POST = HttpMethods.CreatePostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}/todo" (Some todo)
        
    let removeTodoFromList name id =
        let DELETE = HttpMethods.CreateDeleteRequest client Json.strictDeserializer<TodoListDto>
        
        DELETE $"/list/{name}/todo/{id}"

    let updateTodo name todo =
        let PUT = HttpMethods.CreatePutRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        PUT $"/list/{name}/todo" (Some todo)

    let completeTodo name id =
        let POST = HttpMethods.CreatePostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}/todo/{id}/complete" None

    let incompleteTodo name id =
        let POST = HttpMethods.CreatePostRequest client Json.serializeToContent Json.strictDeserializer<TodoListDto>
        
        POST $"/list/{name}/todo/{id}/incomplete" None