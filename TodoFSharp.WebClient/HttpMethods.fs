module internal TodoFSharp.WebClient.HttpMethods

open System.Net.Http
open System.Text

//let GET_Json<'a> (client: HttpClient) (url: string) =
//    async {
//        
//    }

let POST (client: HttpClient) (url: string) =
    async {
        let! response = client.PostAsync(url, null) |> Async.AwaitTask
        return! response.Content.ReadAsStringAsync() |> Async.AwaitTask
    }

let POSTJson (client: HttpClient) (url: string) payload =
     async {
                
        let json = Utils.serialize payload
        let requestBody = new StringContent(json, Encoding.UTF8, "application/json")
        let! response = client.PostAsync(url, requestBody) |> Async.AwaitTask
        
        return! response.Content.ReadAsStringAsync() |> Async.AwaitTask
     }
