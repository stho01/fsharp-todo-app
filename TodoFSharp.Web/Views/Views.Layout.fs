namespace TodoFSharp.Web.Views

open Giraffe.ViewEngine

module Layout =
    let appHeader =
        header [ _classList [bs.P3; bs.BgDark; bs.TextWhite] ] [
            div [ _class bs.Container ] [
                div [ _classList [ bs.DFlex; bs.FlexWrap; bs.AlignItemsCenter ] ] [
                    p [ _classList [bs.FwBold; bs.Fs4; bs.M0] ] [
                        str "To-Do"
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
                       _href $"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"
                       _integrity "sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"
                       _crossorigin "anonymous" ]
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
                
                script [
                    _src "https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"
                    _integrity "sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p"
                    _crossorigin "anonymous"
                ] []
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

