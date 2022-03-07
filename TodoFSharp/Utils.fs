module TodoFSharp.Utils

open System
open System.IO
open System.Text.Json
open Microsoft.Extensions.Logging

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
     
let readAllTextOption (logger: ILogger) fileName =
    try
        match resolveFileName fileName with
        | Ok fileName -> File.ReadAllText fileName |> Some
        | Error err ->
            logger.LogWarning err
            None
    with
    | :? FileNotFoundException as ex ->
        logger.LogError("Failed to read text", ex)
        None

let writeAllText fileName content =
    File.WriteAllText(fileName, content)


let private serializeOptions =
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options

let serialize value =
    JsonSerializer.Serialize(value, serializeOptions)



let deserialize<'a> (json: string) =
    try
        JsonSerializer.Deserialize<'a> json |> Ok
    with
    | ex -> Error ex.Message

let deserializeOption<'a> (json: string) =
    try
        JsonSerializer.Deserialize<'a> json |> Some
    with
    | ex -> None

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