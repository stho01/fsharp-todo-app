module Utils

open System.Text.Json

let defaultSerializerOptions =
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    options

let deserialize<'a> (json: string) =
    JsonSerializer.Deserialize<'a>(json, defaultSerializerOptions)