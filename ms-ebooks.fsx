#I "packages"

#r "FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data

let readLines filePath = File.ReadLines(filePath)

let links = readLines "Ligman_eBooks_2017.txt"

let downloadFile (link : string) =
    async {
        let url = link.Trim()
        let! request = Http.AsyncRequestStream (url = url, httpMethod = "GET", timeout = System.Threading.Timeout.Infinite)
        let fileName = url.Substring(url.LastIndexOf("/"))
        let path = Environment.CurrentDirectory + "\\" + fileName + ".pdf"
        use outputFile = new FileStream(path, FileMode.Create)
        do! request.ResponseStream.CopyToAsync outputFile |> Async.AwaitTask
    }

let go () = 
    links    
    |> Seq.skip 1 
    |> Seq.map (downloadFile)
    |> Async.Parallel
    |> Async.RunSynchronously