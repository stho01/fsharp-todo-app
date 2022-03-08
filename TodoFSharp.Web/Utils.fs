
[<AutoOpen>]
module Utils

open Giraffe.ViewEngine

let _classList (classes: string list) =
    classes |> String.concat " " |> _class


let _checkedBool (isChecked) =
    if isChecked then
        Some _checked
    else
        None