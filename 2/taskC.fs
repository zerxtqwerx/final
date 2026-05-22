module CardCircle

open System.Collections.Generic

// --- Подсчёт частот ---

// Группируем карточки по значению и считаем частоты
// Возвращает seq<int64 * int64> = (значение, частота)
let countFrequencies (cards: int64 seq) : (int64 * int64) seq =
    cards
    |> Seq.groupBy id
    |> Seq.map (fun (value, group) -> (value, Seq.length group |> int64))

// --- Вычисление кандидата для данного доминанта ---

// Для числа a с частотой fa, зная частоты остальных чисел,
// вычисляем максимальное количество карточек в расстановке
let computeCandidate (fa: int64) (freqOthers: (int64 * int64) seq) : int64 =
    let slots = fa / 2L
    
    // Разделяем остальные числа на "безопасные" (freq>=2) и "чистые одиночки" (freq==1)
    let safeFreqs, pureSingles =
        freqOthers
        |> Seq.partition (fun (_, f) -> f >= 2L)
    
    // Параметры безопасного блока
    let pOthers = safeFreqs |> Seq.sumBy (fun (_, f) -> f / 2L)
    let sOthersSafe = safeFreqs |> Seq.sumBy (fun (_, f) -> f % 2L)
    let sPureSingles = pureSingles |> Seq.length |> int64
    
    let safeBlockSize = 2L * pOthers + sOthersSafe
    let safeBlockCount = if safeBlockSize > 0L then 1L else 0L
    let blocksNeeded = safeBlockCount + sPureSingles
    
    if blocksNeeded <= slots then
        // Всё помещается!
        fa + safeBlockSize + sPureSingles
    elif safeBlockCount = 0L then
        // Только чистые одиночки
        fa + min sPureSingles slots
    else
        // 1 слот для безопасного блока, остаток для одиночек
        fa + safeBlockSize + min sPureSingles (slots - 1L)

// --- Решение одного тест-кейса ---

let solve (cards: int64 array) : int64 =
    let n = Array.length cards
    if n < 3 then 0L
    else
        let freqPairs = countFrequencies cards |> Seq.toArray
        
        // Проверяем есть ли хоть одна пара
        let hasPair = freqPairs |> Array.exists (fun (_, f) -> f >= 2L)
        if not hasPair then 0L
        else
            // Перебираем каждое число как доминанта
            let best =
                freqPairs
                |> Seq.filter (fun (_, fa) -> fa >= 2L)
                |> Seq.map (fun (a, fa) ->
                    // Все остальные числа (не a)
                    let freqOthers = freqPairs |> Seq.filter (fun (v, _) -> v <> a)
                    computeCandidate fa freqOthers)
                |> Seq.max
            
            if best >= 3L then best else 0L

// --- Чтение входных данных и запуск ---

let readLine () = System.Console.ReadLine().Trim()

let readCards () =
    let n = readLine () |> int
    readLine().Split(' ') |> Array.map int64, n

let main () =
    let t = readLine () |> int
    
    // Рекурсивная обработка тест-кейсов (без цикла)
    let rec processTests remaining =
        if remaining = 0 then ()
        else
            let cards, _ = readCards ()
            let answer = solve cards
            printfn "%d" answer
            processTests (remaining - 1)
    
    processTests t

// --- Тесты ---

let runTests () =
    let testCases = [|
        ([|1L;1L;1L;3L|], 4L, "1: [1,1,1,3]")
        ([|2L;3L;4L|], 0L, "2: [2,3,4]")
        ([|1L;1L;2L|], 3L, "3: [1,1,2]")
        ([|1L;1L;1L|], 3L, "4: [1,1,1]")
        ([|1L;2L;2L;2L|], 4L, "5: [1,2,2,2]")
        ([|2L;2L;4L;4L|], 4L, "6: [2,2,4,4]")
        ([|1L;1L;2L;2L;3L;3L;3L|], 7L, "7: [1,1,2,2,3,3,3]")
        ([|1L;1L;4L|], 3L, "8: [1,1,4]")
        ([|1L;1L;1L;1L;3L;4L|], 6L, "9: [1,1,1,1,3,4]")
        ([|1L;1L;1L;1L;1L;1L;1L;7L|], 8L, "10: [1,1,1,1,1,1,1,7]")
    |]
    
    testCases
    |> Array.iter (fun (cards, expected, desc) ->
        let got = solve cards
        let status = if got = expected then "PASS" else "FAIL"
        printfn "Test %s: %s (expected=%d, got=%d)" desc status expected got)