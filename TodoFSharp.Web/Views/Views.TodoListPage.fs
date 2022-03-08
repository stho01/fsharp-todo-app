namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine
open TodoFSharp.Web.ViewModels

module TodoListPage =
    let view (model: TodoListPage) =
        [
            div [ _class bs.Container ] [
                Shared.todoListCard model.TodoList
            ]
        ]
        |> Layout.view