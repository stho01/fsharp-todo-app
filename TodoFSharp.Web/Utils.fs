module Utils

open System.Text.Json
open Giraffe.ViewEngine

let _classList (classes: string list) =
    classes |> String.concat " " |> _class
