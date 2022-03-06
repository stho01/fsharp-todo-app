namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine
open TodoFSharp.Web.ViewModels
open Zanaptak.TypedCssClasses
open Utils

type css = CssClasses<"WebRoot/style/bootstrap/dist/css/bootstrap.css", Naming.PascalCase>


module Layout =
    
    let appHeader =
        header [ _classList [css.P3; css.BgDark; css.TextWhite] ] [
            div [ _class css.Container ] [
                div [ _classList [ css.DFlex; css.FlexWrap; css.AlignItemsCenter ] ] [
                    p [ _classList [css.FwBold; css.Fs4; css.M0] ] [
                        str "Todo"
                        span [ _class css.FwLight ] [ str "App" ]
                    ]
                ]
            ]
        ]
    
    let view (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "TodoFSharp.Web" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href $"/style/bootstrap/dist/css/bootstrap.css" ]
            ]
            body [] [
                appHeader
                main [ _class css.Mt4 ] content
                script [ _src "/style/bootstrap/dist/js/bootstrap.js" ] []
            ]
        ]


module Shared =
    let todoListCard (model: TodoList) =
        
        // list items 
        let listItem (index: int) (model: Todo) =
            li [ _classList [ css.ListGroupItem; css.DFlex; css.AlignItemsCenter ] ] [
                input [ _id $"done.[{index}]"; _name "done"; _type "checkbox"; _classList [css.FormCheckInput; css.Me3] ]
                label [ _for $"done.[{index}]" ] [ encodedText model.Todo ]
            ]
            
        let todoLists = model.Todos |> List.mapi listItem
        
        div [ _classList [css.Card] ] [
            div [ _class css.CardHeader ] [
                 h5 [ ] [ encodedText model.Name ]
            ]
            ul [ _classList [css.ListGroup; css.ListGroupFlush] ] todoLists
            div [ _class css.CardBody ] [
                button [ _id "btnSaveTodos"; _classList [css.Btn; css.BtnPrimary] ] [ encodedText "Save" ]                        
            ]
        ]
    

module FrontPage =

    let private todoDetailsLink todo =
        div [] [
            a [ _href $"/list/{todo.Name}"] [ encodedText todo.Name ]
        ]
        
    let private todoDetailsList (todos: TodoList list) =
        
        let row todoList =
            div [ _class css.Col4 ] [
                Shared.todoListCard todoList
            ]
        
        let cards = (todos |> List.map row)
        
        div [ _class css.Row ] cards
    
    let view (model: FrontPage) = 
        [
            div [ _class css.Container ] [
                h1 [] [ encodedText "Todos" ]
                todoDetailsList model.TodoLists.Payload
            ]
        ] |> Layout.view
        
        
module TodoListPage =
    let view (model: TodoListPage) =
        [
            div [ _class css.Container ] [
                Shared.todoListCard model.TodoList
            ]
        ]
        |> Layout.view