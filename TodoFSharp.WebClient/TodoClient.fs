module TodoFSharp.WebClient.TodoClient

open System
open System.Net.Http
open TodoFSharp.WebClient.Dto
open TodoFSharp.WebClient.HttpMethods
open TodoFSharp.WebClient

let responseOf<'TType> = typedefof<'TType> |> Some

let private client =
    let client = new HttpClient()
    let uri = "https://localhost:7157" |> Uri
    client.BaseAddress <- uri
    client


let internal GET url deserializer = GET client deserializer url  
let internal POST url body responseType = POST client url Json.serializeToContent body Json.deserializer<TodoListDto> 


let getTodoLists () = GET "/" Json.deserialize<PagedDataDto<TodoListDto>>
let getTodoList name = GET $"/list/{name}" Json.deserialize<TodoListDto>

let addTodoToList name todo = POST $"/list/{name}/todo" (Some todo) responseOf<TodoListDto>
let removeTodoFromList name id = DELETE<TodoListDto> client $"/list/{name}/todo/{id}"
let updateTodo name todo = PUTJson<TodoListDto> client $"/list/{name}/todo" todo
let completeTodo name id = POST<TodoListDto> client $"/list/{name}/todo/{id}/complete"
let incompleteTodo name id = POST<TodoListDto> client $"/list/{name}/todo/{id}/incomplete"