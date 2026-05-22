module CardCircle

let solve (cards: int64 array) : int64 =
    let n = Array.length cards
    if n < 3 then 0L else
    
    let freq =
        cards
        |> Array.groupBy id
        |> Array.map (fun (v, g) -> (v, Array.length g |> int64))
    
    let hasPair = freq |> Array.exists (fun (_, f) -> f >= 2L)
    if not hasPair then 0L else
    
    let computeCandidate (a: int64, fa: int64) =
        let slots = fa / 2L
        
        let pOthers, sOthersSafe, sPureSingles =
            freq
            |> Array.filter (fun (v, _) -> v <> a)
            |> Array.fold (fun (po, sos, sps) (_, f) ->
                if f >= 2L then
                    (po + f / 2L, sos + f % 2L, sps)
                else
                    (po, sos, sps + 1L)) (0L, 0L, 0L)
        
        let safeBlockSize = 2L * pOthers + sOthersSafe
        let safeBlockCount = if safeBlockSize > 0L then 1L else 0L
        let blocksNeeded = safeBlockCount + sPureSingles
        
        if blocksNeeded <= slots then
            fa + safeBlockSize + sPureSingles
        elif safeBlockCount = 0L then
            fa + min sPureSingles slots
        else
            let extra = max 0L (slots - 1L)
            fa + safeBlockSize + min sPureSingles extra
    
    let best =
        freq
        |> Array.filter (fun (_, f) -> f >= 2L)
        |> Array.map computeCandidate
        |> Array.max
    
    if best >= 3L then best else 0L

[<EntryPoint>]
let main argv =
    let t = System.Console.ReadLine() |> int
    let results =
        [ for _ in 1..t do
            let n = System.Console.ReadLine() |> int
            let cards = System.Console.ReadLine().Split() |> Array.map int64
            yield solve cards ]
    results |> List.iter (printfn "%d")
    0