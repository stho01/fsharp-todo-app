﻿module TodoFSharp.Web.HttpHandlers

open Microsoft.AspNetCore.Http
open TodoFSharp.Web.Dto
open TodoFSharp.Web.ViewModels
open TodoFSharp.Web.Views
open Giraffe
open TodoFSharp.WebClient

let indexHandler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let pageResult =
            TodoClient.getTodoLists ()
            |> Async.RunSynchronously
            // |> (PagedResultDto.toViewModel TodoListDto.toViewModel)
        
        let a = pageResult |> (PagedResultDto.toViewModel TodoListDto.toViewModel)
        
        let view = FrontPage.view { TodoLists = a }
        
        htmlView view next ctx

let todoListHandler name : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let list =
            TodoClient.getTodoList name
            |> Async.RunSynchronously
            |> TodoListDto.toViewModel
        
        let view = TodoListPage.view { TodoList = list }
        
        htmlView view next ctx