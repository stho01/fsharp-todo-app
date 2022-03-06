module TodoFSharp.Web.HttpHandlers

open Microsoft.AspNetCore.Http
open TodoFSharp.Web.ViewModels
open TodoFSharp.Web.Views
open Giraffe

let indexHandler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        
        let todos =
            TodoServiceClient.getTodoLists ()
            |> Async.RunSynchronously
            |> Utils.deserialize<TodoListDetails list>
        
        let view = FrontPage.view { TodoLists = todos }
        
        htmlView view next ctx

let todoListHandler name : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let list =
            TodoServiceClient.getTodoList name
            |> Async.RunSynchronously
            |> Utils.deserialize<TodoList>
        
        let view = TodoListPage.view { TodoList = list }
        
        htmlView view next ctx