:- use_module(library(aggregate)).
:- use_module(library(clpfd)).

count_freq([], []).
count_freq([H|T], [(H,1)|Rest]) :-
    count_freq(T, Rest1),
    (member((H,Old), Rest1) ->
        select((H,Old), Rest1, Rest2),
        New is Old + 1,
        Rest = [(H,New)|Rest2]
    ;
        Rest = [(H,1)|Rest1]
    ).

compute_candidate(Fa, POthers, SOthersSafe, SPureSingles, Candidate) :-
    Slots is Fa // 2,
    SafeBlockSize is 2 * POthers + SOthersSafe,
    (SafeBlockSize > 0 -> SafeBlockCount = 1 ; SafeBlockCount = 0),
    BlocksNeeded is SafeBlockCount + SPureSingles,
    (BlocksNeeded =< Slots ->
        Candidate is Fa + SafeBlockSize + SPureSingles
    ; SafeBlockCount =:= 0 ->
        (SPureSingles =< Slots -> Candidate is Fa + SPureSingles ; Candidate is Fa + Slots)
    ;
        ExtraSlots is max(0, Slots - 1),
        (SPureSingles =< ExtraSlots -> Candidate is Fa + SafeBlockSize + SPureSingles ; Candidate is Fa + SafeBlockSize + ExtraSlots)
    ).

others_stats([], _, 0, 0, 0).
others_stats([(V,F)|Rest], Exclude, POthers, SOthersSafe, SPureSingles) :-
    V = Exclude,
    !,
    others_stats(Rest, Exclude, POthers, SOthersSafe, SPureSingles).
others_stats([(_,F)|Rest], Exclude, POthers, SOthersSafe, SPureSingles) :-
    F >= 2,
    !,
    others_stats(Rest, Exclude, POthers1, SOthersSafe1, SPureSingles1),
    POthers is POthers1 + F // 2,
    SOthersSafe is SOthersSafe1 + F mod 2,
    SPureSingles = SPureSingles1.
others_stats([(_,1)|Rest], Exclude, POthers, SOthersSafe, SPureSingles) :-
    others_stats(Rest, Exclude, POthers, SOthersSafe, SPureSingles1),
    SPureSingles is SPureSingles1 + 1.

solve_cards(Cards, Answer) :-
    length(Cards, N),
    N < 3,
    !,
    Answer = 0.
solve_cards(Cards, Answer) :-
    count_freq(Cards, FreqPairs),
    aggregate_all(max(Cand), (member((_,Fa), FreqPairs), Fa >= 2,
        (foreach((V,Fa1), FreqPairs), fromto(0,POIn,POOut,PO),
         fromto(0,SOSIn,SOSOut,SOS), fromto(0,SPSIn,SPSOut,SPS),
         (V \= a -> true ; (Fa2 is Fa1, others_stats...))),
        compute_candidate(Fa, PO, SOS, SPS, Cand)), MaxCand),
    (MaxCand >= 3 -> Answer = MaxCand ; Answer = 0).