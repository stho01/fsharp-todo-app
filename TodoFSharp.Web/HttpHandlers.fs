module TodoFSharp.Web.HttpHandlers

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
        
        
let addTodoToListHandler name: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! todo = ctx.BindJsonAsync<NewTodoDto>()
            let a = name
            
            return! json $"""{{ "Todo": "{todo.Todo}" }}""" next ctx    
        }