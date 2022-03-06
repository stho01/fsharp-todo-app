namespace TodoFSharp.Web.ViewModels

open System

type PagedData<'TItem> =
    { Page: int
      Total: int
      Payload: 'TItem list }

type Todo =
    { Id: Guid
      Todo: string
      Done: bool
      CreatedDate: DateTimeOffset }

type TodoList =
    { Name: string
      Todos: Todo list }

type FrontPage =
    { TodoLists: PagedData<TodoList> }
    
type TodoListPage =
    { TodoList: TodoList }