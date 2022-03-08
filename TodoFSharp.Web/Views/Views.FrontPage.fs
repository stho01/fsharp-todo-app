namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine
open TodoFSharp.Web.ViewModels

module FrontPage =

    let private todoDetailsLink todo =
        div [] [
            a [ _href $"/list/{todo.Name}"] [ encodedText todo.Name ]
        ]
        
    let private todoDetailsList (todos: TodoList list) =
        
        let row todoList =
            div [ _class bs.Col4 ] [
                Shared.todoListCard todoList
            ]
        
        let cards = (todos |> List.map row)
        
        div [ _class bs.Row ] cards
    
    let view (model: FrontPage) = 
        [
            div [ _class bs.Container ] [
                todoDetailsList model.TodoLists.Payload
            ]
        ] |> Layout.view
        


