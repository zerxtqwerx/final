module Solution

let solveOne n a b =
    let posA = Array.create (n + 2) 0
    let posB = Array.create (n + 2) 0
    for i in 1..n do
        posA.[a.[i-1]] <- i
        posB.[b.[i-1]] <- i

    let nextSameA = Array.create (n + 2) (n + 1)
    let prevSameA = Array.create (n + 2) 0
    let nextSameB = Array.create (n + 2) (n + 1)
    let prevSameB = Array.create (n + 2) 0

    let buildPrevNext arr nextSame prevSame =
        let lastNext = Array.create (n + 2) (n + 1)
        let lastPrev = Array.create (n + 2) 0
        for i = n downto 1 do
            nextSame.[i] <- lastNext.[arr.[i-1]]
            lastNext.[arr.[i-1]] <- i
        for i = 1 to n do
            prevSame.[i] <- lastPrev.[arr.[i-1]]
            lastPrev.[arr.[i-1]] <- i

    buildPrevNext a nextSameA prevSameA
    buildPrevNext b nextSameB prevSameB

    let maxReachL = Array.create (n + 2) 0
    let lastPosForVal = Array.create (n + 2) 0

    let rec processPos i =
        if i > n then ()
        else
            maxReachL.[i] <- i
            if a.[i-1] <> b.[i-1] then
                let va = a.[i-1]
                let vb = b.[i-1]
                if lastPosForVal.[va] > 0 || lastPosForVal.[vb] > 0 then
                    maxReachL.[i] <- max maxReachL.[i] (max lastPosForVal.[va] lastPosForVal.[vb] + 1)
                if prevSameA.[i] > 0 && b.[prevSameA.[i]-1] = b.[i-1] then
                    maxReachL.[i] <- max maxReachL.[i] (prevSameA.[i] + 1)
                if prevSameB.[i] > 0 && a.[prevSameB.[i]-1] = a.[i-1] then
                    maxReachL.[i] <- max maxReachL.[i] (prevSameB.[i] + 1)
                if posA.[vb] > 0 && posA.[vb] < i && b.[posA.[vb]-1] = va then
                    maxReachL.[i] <- max maxReachL.[i] (posA.[vb] + 1)
                if posB.[va] > 0 && posB.[va] < i && a.[posB.[va]-1] = vb then
                    maxReachL.[i] <- max maxReachL.[i] (posB.[va] + 1)
                lastPosForVal.[va] <- i
                lastPosForVal.[vb] <- i
            processPos (i + 1)

    processPos 1

    let rec computeAns r curMin acc =
        if r > n then acc
        else
            let newMin = max curMin maxReachL.[r]
            computeAns (r + 1) newMin (acc + int64 (r - newMin + 1))

    computeAns 1 1 0L

[<EntryPoint>]
let main _ =
    let t = int (stdin.ReadLine())
    let results = Array.init t (fun _ ->
        let n = int (stdin.ReadLine())
        let a = stdin.ReadLine().Split() |> Array.map int
        let b = stdin.ReadLine().Split() |> Array.map int
        solveOne n a b)
    Array.iter (printfn "%d") results
    0