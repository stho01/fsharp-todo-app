module Utils

open System.Text.Json
open Giraffe.ViewEngine

let defaultSerializerOptions =
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    options

let deserialize<'a> (json: string) =
    JsonSerializer.Deserialize<'a>(json, defaultSerializerOptions)
    
let _classList (classes: string list) =
    classes |> String.concat " " |> _class
