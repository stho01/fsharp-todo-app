
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


let partial f a b = f(a, b)
let stringJoin (sep: string) (list: 'a list) =
    String.Join(sep, list)

let mergeAttr (attributes: XmlAttribute list list) =
    let values group =
        let key = fst group
        let values =
            group
            |> snd
            |> List.map snd
            |> List.distinct
            |> stringJoin " "
            
        XmlAttribute.KeyValue (key, values) 
        
    attributes
    |> List.concat
    |> List.choose keyValue
    |> List.groupBy fst
    |> List.map values