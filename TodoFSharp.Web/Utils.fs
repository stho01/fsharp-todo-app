
[<AutoOpen>]
module Utils

open System
open Giraffe.ViewEngine
open Microsoft.FSharp.Collections

let _classList (classes: string list) =
    classes |> String.concat " " |> _class

let _checkedBool (isChecked) =
    if isChecked then
        Some _checked
    else
        None
        
let private keyValue item =
    match item with
    | XmlAttribute.KeyValue (k, v) -> (k, v) |> Some
    | _ -> None

let private mergeAttribute (group, (key: string, value: string) list =
    value.Split " "
    |> Array.distinct
    
let private mergeAttributes (attributes: XmlAttribute list) =
    attributes
    |> List.choose keyValue
    |> List.groupBy fst
    |> List.map (fun group ->
        let values = group |> snd |> List.map snd |>  
        (fst group, values))