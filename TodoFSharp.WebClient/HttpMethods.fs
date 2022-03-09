module internal TodoFSharp.WebClient.HttpMethods

open System.Net.Http
open System.Text
open TodoFSharp.WebClient.Serialization

(*
    Generic Http Method calls. 
*)

let emptySerializer : Deserializer<string> = id

let GET (client: HttpClient) (deserializer: Deserializer<'TResponse>) (url: string) =
    async {
        let! response = client.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        return content |> deserializer
    }

//let GET<'TResponse> (client: HttpClient) (url: string) =
//    async {
//        let! response = client.GetAsync(url) |> Async.AwaitTask
//        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
//        return content |> Utils.deserialize<'TResponse>
//    }

let POST
    (client: HttpClient)
    (url: string)
    (serializer: Serializer<'TPayload>)
    payload
    (deserializer: Deserializer<'TResponse>) =
        async {
            let payload =
                match payload with
                | Some payload -> serializer payload
                | None -> null
            
            let! response = client.PostAsync(url, payload) |> Async.AwaitTask
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            
            return content |> deserializer
        }


//let POST<'TResponse> (client: HttpClient) (url: string) =
//    async {
//        let! response = client.PostAsync(url, null) |> Async.AwaitTask
//        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
//        return content |> Utils.deserialize<'TResponse>
//    }

//let POSTJson<'TResponse> (client: HttpClient) (url: string) payload =
//     async {
//        let json = Utils.serialize payload
//        let requestBody = new StringContent(json, Encoding.UTF8, "application/json")
//        let! response = client.PostAsync(url, requestBody) |> Async.AwaitTask
//        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
//        return content |> Utils.deserialize<'TResponse>
//     }
     
let DELETE<'TResponse> (client: HttpClient) (url: string) =
    async {
        let! response = client.DeleteAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'TResponse>
    }

let PUTJson<'TResponse> (client: HttpClient) (url: string) payload =
    async {
        let json = Utils.serialize payload
        let requestBody = new StringContent(json, Encoding.UTF8, "application/json")
        let! response = client.PutAsync(url, requestBody) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'TResponse>
    }