module main
open RestSharp
open ResourceAdmin

[<EntryPoint>]
let main argv = 
    let auth = new HttpBasicAuthenticator("guest", "guest")
    let client = new RestClient "http://localhost:15672"
    client.Authenticator <- auth
    
    //ResourceAdmin.clearOutQueues client
    let processes = ClusterAdmin.createCluster 2 ClusterAdmin.simpleNodeStrategy
    Seq.iter (fun _ -> ()) processes
    //ResourceAdmin.setupFederation auth  |> ignore

    0 // return an integer exit code