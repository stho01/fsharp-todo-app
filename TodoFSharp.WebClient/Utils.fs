module internal TodoFSharp.WebClient.Utils

open System.Text.Json

let private serializeOptions =
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    options

let serialize value = JsonSerializer.Serialize(value, serializeOptions)
let deserialize<'a> (json: string) = JsonSerializer.Deserialize<'a>(json, serializeOptions) 
