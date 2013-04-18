open System
open RestSharp

let e = Uri.EscapeDataString

type RabbitMQResource() =
    member val name = "" with get, set
    member val vhost = "" with get, set

    member this.url resourceType =
        "/api/" + resourceType + "/" + e(this.vhost) + "/" + e(this.name)

let listResources (client:RestClient) resourceType =
    client.Execute<ResizeArray<RabbitMQResource>>(new RestRequest ("/api/" + resourceType))

let deleteResource (client:RestClient) resourceType (resources:RabbitMQResource seq) =
    resources |> Seq.iter (fun resource -> client.Execute (new RestRequest(resource.url resourceType, Method.DELETE)) |> ignore)

[<EntryPoint>]
let main argv = 
    let client = new RestClient "http://localhost:15672"
    client.Authenticator <- new HttpBasicAuthenticator("guest", "guest")
    (* Delete all exchanges with names starting with "NServiceBus" *)
    let exchanges = listResources client "exchanges"
    exchanges.Data |> Seq.filter (fun x -> x.name.StartsWith "NServiceBus") |> deleteResource client "exchanges"
    (* Print all queue names *)
    (listResources client "queues").Data |> Seq.map (fun x-> x.name) |> Seq.iter (printfn "%s")
    0 // return an integer exit code