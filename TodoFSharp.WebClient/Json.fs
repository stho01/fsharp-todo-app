module TodoFSharp.WebClient.Json

open System.Net.Http
open System.Text
open System.Text.Json
open TodoFSharp.WebClient.Serialization

let private serializeOptions =
    let options = JsonSerializerOptions()
    options.WriteIndented <- true
    options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    options

// Deserialization

let anonymousDeserializer : Deserializer =
    fun returnType json -> JsonSerializer.Deserialize(json, returnType, serializeOptions)

let strictDeserializer<'TResponse> : Deserializer<'TResponse> =
    fun json -> JsonSerializer.Deserialize<'TResponse>(json, serializeOptions)

let rec deserialize<'TResponse> (json: string) =
    strictDeserializer<'TResponse> json

// Serialization

let serialize value = JsonSerializer.Serialize(value, serializeOptions)
let toHttpContent json =
    new StringContent(json, Encoding.UTF8, "application/json")
    :> HttpContent
    
let serializeToContent<'TIn> : Serialization.Serializer<'TIn> = serialize >> toHttpContent