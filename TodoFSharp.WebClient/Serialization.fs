namespace TodoFSharp.WebClient.Serialization

open System.Net.Http

type Serializer<'TIn> = 'TIn -> HttpContent
type Deserializer<'TOut> = string -> 'TOut

