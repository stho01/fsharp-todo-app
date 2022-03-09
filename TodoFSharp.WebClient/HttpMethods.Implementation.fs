module internal TodoFSharp.WebClient.HttpMethods

open System.Net.Http
open Microsoft.FSharp.Control
open TodoFSharp.WebClient.Serialization

(*
    # Implementation
    Generic Http Method calls.
*)


let createGetRequest<'TResponse>
    (client: HttpClient)
    (deserializer: Deserializer<'TResponse>)
    : HttpGet<'TResponse> =
        fun url -> 
            async {
                let! response = client.GetAsync(url) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                return deserializer content
            }

let createPostRequest
    (client: HttpClient)
    (serializer: Serializer<'TPayload>)
    (deserializer: Deserializer<'TResponse>)
    : HttpPost<'TPayload, 'TResponse> =
        fun url payload ->
            async {
                let serializedPayload =
                    match payload with
                    | Some p -> serializer p
                    | _ -> null
                
                let! response = client.PostAsync(url, serializedPayload) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                return deserializer content
            }


let createDeleteRequest
    (client: HttpClient)
    (deserializer: Deserializer<'TResponse>)
    : HttpDelete<'TResponse> =
        fun url ->
            async {
                let! response = client.DeleteAsync(url) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                return deserializer content 
            }


let createPutRequest
    (client: HttpClient)
    (serializer: Serializer<'TPayload>)
    (deserializer: Deserializer<'TResponse>)
    : HttpPut<'TPayload, 'TResponse> =
        fun  url payload ->
            async {
                let serializedPayload =
                    match payload with
                    | Some p -> serializer p
                    | _ -> null
                    
                let! response = client.PutAsync(url, serializedPayload) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                return deserializer content 
            }