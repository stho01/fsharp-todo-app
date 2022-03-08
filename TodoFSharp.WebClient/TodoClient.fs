module TodoFSharp.WebClient.TodoClient

open System
open System.Net.Http
open TodoFSharp.WebClient.Dto
open TodoFSharp.WebClient.HttpMethods

let private client =
    let client = new HttpClient()
    let uri = "https://localhost:7157" |> Uri
    client.BaseAddress <- uri
    client

let getTodoLists () = GET<PagedDataDto<TodoListDto>> client "/"
let getTodoList name = GET<TodoListDto> client $"/list/{name}"
let addTodoToList name todo = POSTJson<TodoListDto> client $"/list/{name}/todo" todo
let removeTodoFromList name id = DELETE<TodoListDto> client $"/list/{name}/todo/{id}"
let updateTodo name todo = PUTJson<TodoListDto> client $"/list/{name}/todo" todo
let completeTodo name id = POST<TodoListDto> client $"/list/{name}/todo/{id}/complete"
let incompleteTodo name id = POST<TodoListDto> client $"/list/{name}/todo/{id}/incomplete"