module TodoFSharp.Utils

open System
open System.IO
open System.Text.Json

let private resolveFileName fileName =
    match box fileName with
    | :? string as fileName -> fileName |> Ok
    | :? FileInfo as fileInfo -> fileInfo.FullName |> Ok
    | _ -> Error "Unrecognized argument type"

let readAllText fileName =
    try
        match resolveFileName fileName with
        | Ok fileName -> File.ReadAllText fileName |> Ok
        | Error err -> Error err
    with
    | :? FileNotFoundException -> Error "Todo list not found"
     
let writeAllText fileName content =
    File.WriteAllText(fileName, content)


let private serializeOptions =
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options

let serialize value =
    JsonSerializer.Serialize(value, serializeOptions)

let deserialize<'a> (json: Result<string, string>) =
    match json with
    | Ok json -> JsonSerializer.Deserialize<'a> json |> Ok
    | Error err -> Error err

let deserializeOption<'a> (json: Result<string, string>) =
    match json with
    | Ok json -> JsonSerializer.Deserialize<'a> json |> Some
    | Error _ -> None

let jsonFile fileName =
        let fileInfo = fileName |> FileInfo
        match fileInfo.Extension with
        | ".json" -> Some fileInfo
        | _ -> None
        
let fileNameWithoutExtension (fileInfo: FileInfo) =
    fileInfo.FullName
    |> Path.GetFileNameWithoutExtension
    
let fileName (fileInfo: FileInfo) = fileInfo.FullName


let min (a: int) (b:int) = Math.Min(a, b)