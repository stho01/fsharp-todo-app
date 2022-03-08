﻿namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine
open TodoFSharp.Web.ViewModels
open Zanaptak.TypedCssClasses

type bs = CssClasses<"WebRoot/style/bootstrap/dist/css/bootstrap.css", Naming.PascalCase>
type fa = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css", Naming.PascalCase>
type css = CssClasses<"WebRoot/style/style.css", Naming.PascalCase>

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
        
        div [ _classList [bs.Card; "todo-list-card"; bs.Mb2] ] [
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