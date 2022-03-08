module TodoFSharp.Web.HttpHandlers

open System
open Microsoft.AspNetCore.Http
open TodoFSharp.Web.Dto
open TodoFSharp.Web.ViewModels
open TodoFSharp.Web.Views
open Giraffe
open TodoFSharp.WebClient
open TodoFSharp.WebClient.Dto

let indexHandler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let pageResult =
            TodoClient.getTodoLists ()
            |> Async.RunSynchronously
            |> (PagedResultDto.toViewModel TodoListDto.toViewModel)
        
        let view = FrontPage.view { TodoLists = pageResult }
        
        htmlView view next ctx

let todoListHandler name : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let list =
            TodoClient.getTodoList name
            |> Async.RunSynchronously
            |> TodoListDto.toViewModel
        
        let view = TodoListPage.view { TodoList = list }
        
        htmlView view next ctx
        
        
let addTodoToListHandler name : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! todo = ctx.BindJsonAsync<NewTodoDto>()
            let newTodo : TodoDto = {
                Id = Some todo.Id
                Todo = todo.Todo
                Done = Some false
                CreatedDate = None
            }
            
            TodoClient.addTodoToList name newTodo
            |> Async.RunSynchronously
            |> ignore
                
            ctx.SetStatusCode StatusCodes.Status204NoContent
                
            return! next ctx    
        }
        
let removeTodoFromList (name: string, id: string) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let guid = id |> Guid.Parse
            
            TodoClient.removeTodoFromList name guid
            |> Async.RunSynchronously
            |> ignore
            
            return! next ctx
        }
        
let completeTodo (name: string, id: string): HttpHandler =
    fun next ctx ->
        task {
            
            TodoClient.completeTodo name id
            |> Async.RunSynchronously
            |> ignore
            
            return! next ctx
        }
        
let incompleteTodo (name: string, id: string): HttpHandler =
    fun next ctx ->
        task {
            
            TodoClient.incompleteTodo name id
            |> Async.RunSynchronously
            |> ignore
            
            return! next ctx
        }