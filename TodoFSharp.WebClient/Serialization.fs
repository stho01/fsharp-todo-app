namespace TodoFSharp.WebClient.Serialization

open System
open System.Net.Http

type Serializer<'TIn> = 'TIn -> HttpContent
type Deserializer<'TOut> = string -> 'TOut
type Deserializer = Type -> string -> obj