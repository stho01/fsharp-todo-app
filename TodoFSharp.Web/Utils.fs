
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


let mergeAttributes (attributes: XmlAttribute list) =
    let values group =
        let key = fst group
        let values = group |> snd |> List.map snd |> List.distinct
        XmlAttribute.KeyValue (key, values) 
        
    attributes
    |> List.choose keyValue
    |> List.groupBy fst
    |> List.map values
    