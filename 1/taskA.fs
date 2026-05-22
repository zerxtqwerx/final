let solve (n: int64) (a: int64) (b: int64) =
    let full = n / 3L
    let rem  = n % 3L
    let base' = full * (min b (3L * a))
    let extra =
        match rem with
        | 0L -> 0L
        | 1L -> min a b
        | _  -> min (2L * a) b
    base' + extra

[<EntryPoint>]
let main _ =
    let t = System.Console.ReadLine() |> int
    for _ in 1 .. t do
        let p = System.Console.ReadLine().Split(' ')
        printfn "%d" (solve (int64 p[0]) (int64 p[1]) (int64 p[2]))
    0