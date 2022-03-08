namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine

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

