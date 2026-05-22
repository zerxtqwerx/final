:- use_module(library(lists)).
:- use_module(library(aggregate)).

count_freq(List, FreqPairs) :-
    msort(List, Sorted),
    run_length(Sorted, FreqPairs).

run_length([], []).
run_length([H|T], [(H, Count)|Rest]) :-
    count_prefix(H, [H|T], Count, Remaining),
    run_length(Remaining, Rest).

count_prefix(_, [], 0, []).
count_prefix(X, [X|T], Count, Remaining) :-
    !,
    count_prefix(X, T, C1, Remaining),
    Count is C1 + 1.
count_prefix(_, List, 0, List).

% compute_candidate(+Fa, +POthers, +SOthersSafe, +SPureSingles, -Candidate)
compute_candidate(Fa, POthers, SOthersSafe, SPureSingles, Candidate) :-
    Slots is Fa // 2,
    SafeBlockSize is 2 * POthers + SOthersSafe,
    (SafeBlockSize > 0 -> SafeBlockCount = 1 ; SafeBlockCount = 0),
    BlocksNeeded is SafeBlockCount + SPureSingles,
    (BlocksNeeded =< Slots ->
        Candidate is Fa + SafeBlockSize + SPureSingles
    ; SafeBlockCount =:= 0 ->
        Candidate is Fa + min(SPureSingles, Slots)
    ;
        ExtraSlots is Slots - 1,
        Candidate is Fa + SafeBlockSize + min(SPureSingles, ExtraSlots)
    ).

min(A, B, A) :- A =< B, !.
min(_, B, B).

solve(Cards, Answer) :-
    length(Cards, N),
    (N < 3 ->
        Answer = 0
    ;
        count_freq(Cards, FreqPairs),
        (member((_, F), FreqPairs), F >= 2 ->
            best_candidate(FreqPairs, FreqPairs, 0, Best),
            (Best >= 3 -> Answer = Best ; Answer = 0)
        ;
            Answer = 0
        )
    ).

best_candidate(_, [], Best, Best).
best_candidate(AllFreqs, [(_, Fa)|Rest], CurBest, FinalBest) :-
    (Fa < 2 ->
        best_candidate(AllFreqs, Rest, CurBest, FinalBest)
    ;
        others_stats(AllFreqs, Fa, 0, 0, 0, POthers, SOthersSafe, SPureSingles),
        compute_candidate(Fa, POthers, SOthersSafe, SPureSingles, Candidate),
        NewBest is max(CurBest, Candidate),
        best_candidate(AllFreqs, Rest, NewBest, FinalBest)
    ).

% Более правильная версия: перебираем пары (Key, Freq)
solve2(Cards, Answer) :-
    length(Cards, N),
    (N < 3 ->
        Answer = 0
    ;
        count_freq(Cards, FreqPairs),
        aggregate_all(max(Cand), 
            dominant_candidate(FreqPairs, FreqPairs, Cand),
            MaxCand),
        (MaxCand >= 3 -> Answer = MaxCand ; Answer = 0)
    ).

dominant_candidate(AllFreqs, [(Val, Fa)|_], Candidate) :-
    Fa >= 2,
    pairs_others(AllFreqs, Val, POthers, SOthersSafe, SPureSingles),
    Slots is Fa // 2,
    SafeBlockSize is 2 * POthers + SOthersSafe,
    (SafeBlockSize > 0 -> SafeBlockCount = 1 ; SafeBlockCount = 0),
    BlocksNeeded is SafeBlockCount + SPureSingles,
    (BlocksNeeded =< Slots ->
        Candidate is Fa + SafeBlockSize + SPureSingles
    ; SafeBlockCount =:= 0 ->
        Candidate is Fa + min(SPureSingles, Slots)
    ;
        ExtraSlots is Slots - 1,
        Candidate is Fa + SafeBlockSize + min(SPureSingles, ExtraSlots)
    ).
dominant_candidate(AllFreqs, [_|Rest], Candidate) :-
    dominant_candidate(AllFreqs, Rest, Candidate).

% pairs_others(+FreqPairs, +ExcludeVal, -POthers, -SOthersSafe, -SPureSingles)
pairs_others([], _, 0, 0, 0).
pairs_others([(V, _)|Rest], ExcV, PO, SOS, SPS) :-
    V == ExcV, !,
    pairs_others(Rest, ExcV, PO, SOS, SPS).
pairs_others([(_, F)|Rest], ExcV, PO, SOS, SPS) :-
    pairs_others(Rest, ExcV, PO1, SOS1, SPS1),
    (F >= 2 ->
        PO is PO1 + F // 2,
        SOS is SOS1 + F mod 2,
        SPS = SPS1
    ;
        PO = PO1,
        SOS = SOS1,
        SPS is SPS1 + 1
    ).

others_stats([], _, PO, SOS, SPS, PO, SOS, SPS).
others_stats([(_, F)|Rest], ExcF, PO0, SOS0, SPS0, PO, SOS, SPS) :-
    others_stats(Rest, ExcF, PO0, SOS0, SPS0, PO, SOS, SPS).

% --- Главная функция ---

% solve_main/0: читаем из stdin
solve_main :-
    read_term(T, []), % количество тест-кейсов
    NumTests is T,
    solve_tests(NumTests).

solve_tests(0) :- !.
solve_tests(N) :-
    N > 0,
    read_term(NumCards, []),
    read_cards(NumCards, Cards),
    solve2(Cards, Ans),
    write(Ans), nl,
    N1 is N - 1,
    solve_tests(N1).

read_cards(0, []) :- !.
read_cards(N, [C|Rest]) :-
    N > 0,
    read_term(C, []),
    N1 is N - 1,
    read_cards(N1, Rest).