namespace TodoFSharp.Web.Views

open System
open Giraffe.ViewEngine
open TodoFSharp.Web.ViewModels

module Layout =
    let view (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "TodoFSharp.Web" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href $"/style/bootstrap/dist/css/bootstrap.css" ]
            ]
            body [] [
                header [ _class "app-header" ] [
                    div [ _class "app-header__content" ] [
                        encodedText "Todo"
                        span [ _class "text-thin" ] [ encodedText "App" ]    
                    ]
                ]
                main [] content
                script [ _src "/style/bootstrap/dist/js/bootstrap.js" ] []
            ]
        ]

module FrontPage =

    let private todoDetailsLink todo =
        div [] [
            a [ _href $"/list/{todo.Name}"] [ encodedText todo.Name ]
        ]
        
    let private todoDetailsList (todos: TodoListDetails list) =
        let links = (todos |> List.map todoDetailsLink)
        
        div [] links
    
    let view (model: FrontPage) = 
        [
            div [ _class "container" ] [
                h1 [] [ encodedText "Todos" ]
                todoDetailsList model.TodoLists
            ]
        ] |> Layout.view
        
        
module TodoListPage =
    let private todoRow (index: int) (model: Todo) =
        li [ _class "list-group-item d-flex align-items-center" ] [
            input [ _id $"done.[{index}]"; _name "done"; _type "checkbox"; _class "form-check-input me-3" ]
            label [ _for $"done.[{index}]" ] [ encodedText model.Todo ]
        ]
    
    let view (model: TodoListPage) =
        let todoLists = model.TodoList.Todos |> List.mapi todoRow
        [
            div [ _class "container" ] [
                div [ _class "card pa-5"; _style "width: 450px"] [
                    div [ _class "card-header" ] [
                         h5 [ ] [ encodedText model.TodoList.Name ]
                    ]
                    ul [ _class "list-group list-group-flush" ] todoLists
                    div [ _class "card-body" ] [
                        button [ _id "btnSaveTodos"; _class "btn btn-primary" ] [ encodedText "Save" ]                        
                    ]
                ]
            ]
        ]
        |> Layout.view