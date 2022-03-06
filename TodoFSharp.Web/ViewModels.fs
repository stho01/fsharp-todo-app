namespace TodoFSharp.Web.ViewModels

open System

type Todo =
    { Id: Guid
      Todo: string
      Done: bool
      CreatedDate: DateTimeOffset }

type TodoList =
    { Name: string
      Todos: Todo list }

type TodoListDetails =
    { Name: string
      NumberOfTodos: int }
    
type FrontPage =
    { TodoLists: TodoListDetails list }
    
    
    
type TodoListPage =
    { TodoList: TodoList }