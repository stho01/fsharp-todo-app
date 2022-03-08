namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine
open TodoFSharp.Web.ViewModels
open Zanaptak.TypedCssClasses
open Utils

type bs = CssClasses<"WebRoot/style/bootstrap/dist/css/bootstrap.css", Naming.PascalCase>
type fa = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css", Naming.PascalCase>
type css = CssClasses<"WebRoot/style/style.css", Naming.PascalCase>


module Layout =
    
    let appHeader =
        header [ _classList [bs.P3; bs.BgDark; bs.TextWhite] ] [
            div [ _class bs.Container ] [
                div [ _classList [ bs.DFlex; bs.FlexWrap; bs.AlignItemsCenter ] ] [
                    p [ _classList [bs.FwBold; bs.Fs4; bs.M0] ] [
                        str "Todo"
                        span [ _class bs.FwLight ] [ str "App" ]
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
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href $"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href $"/style/style.css" ]
            ]
            body [] [
                appHeader
                main [ _class bs.Mt4 ] content
                script [ _src "/style/bootstrap/dist/js/bootstrap.js" ] []
                script [ _type "module" ] [
                    rawText """
                        import TodoListCard from './scripts/todo-list-card.js';
                        const todoListContainers = [...document.querySelectorAll('.todo-list-card')];
                        const cards = todoListContainers.map(container => {
                            const todoList = new TodoListCard(container);
                            todoList.init();
                            return todoList;
                        })
                    """
                ]
            ]
        ]

module Shared =
    
    let spacer = div [ _class bs.FlexGrow1 ] [] 
    
    let todoListCard (model: TodoList) =
        
        // list items 
        let listItem (isDone: bool) (index: int) (model: Todo) =
            li [ _classList [ bs.ListGroupItem; bs.DFlex; bs.AlignItemsCenter ] ] [
                input ([ _id (model.Id.ToString()); _name "done"; _type "checkbox"; _classList [bs.FormCheckInput; bs.Me3] ] @ (if isDone then [_checked] else []))
                label [ _for (model.Id.ToString()) ] [ encodedText model.Todo ]
                spacer
                button [ _classList [ "todo-list-card__remove"; bs.Btn; css.BtnIcon; css.BtnSm; bs.TextMuted ] ] [
                    i [ _classList [fa.Fa; fa.FaClose] ] []
                ]
            ]
 
        let isDone (todo: Todo) = todo.Done = true
        let notDone (todo: Todo) = todo.Done = false
        
        let todoLists =
            model.Todos
            |> List.where notDone
            |> List.mapi (listItem false)
            
        let completed =
            model.Todos
            |> List.where isDone
            |> List.mapi (listItem true)
        
        div [ _classList [bs.Card; "todo-list-card"] ] [
            input [ _class "todo-list-card__name"; _type "hidden"; _value model.Name ]
            div [ _class bs.CardHeader ] [
                 h5 [ ] [ encodedText model.Name ]
            ]
            ul [ _classList [bs.ListGroup; bs.ListGroupFlush; "todo-list-card__todos"] ] todoLists
            div [ _classList [bs.Px3; bs.Py1; bs.DFlex; bs.AlignItemsCenter] ] [
                i [ _classList [bs.TextMuted; fa.Fa; fa.FaPlus; bs.Me3 ] ] []
                input [ _type "text"; _classList [bs.FormControlPlaintext; bs.Px2; bs.FormControlSm; bs.W100; "todo-list-card__new"]; _placeholder "Listeelement"]
                
            ]
            ul [ _classList [bs.ListGroup; bs.ListGroupFlush; "todo-list-card__completed"] ] completed
        ]
    

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
        
        
module TodoListPage =
    let view (model: TodoListPage) =
        [
            div [ _class bs.Container ] [
                Shared.todoListCard model.TodoList
            ]
        ]
        |> Layout.view