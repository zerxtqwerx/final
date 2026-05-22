:- initialization(main).

main :-
    read_line_to_string(user_input, Line),
    number_string(T, Line),
    process(T),
    halt.

process(0) :- !.
process(T) :-
    T > 0,
    read_line_to_string(user_input, Line),
    split_string(Line, " ", " ", Parts),
    maplist(number_string, [N, A, B], Parts),
    Full is N // 3,
    Rem  is N mod 3,
    Base is Full * min(B, 3 * A),
    extra(Rem, A, B, Extra),
    Cost is Base + Extra,
    writeln(Cost),
    T1 is T - 1,
    process(T1).

extra(0, _, _, 0).
extra(1, A, B, E) :- E is min(A, B).
extra(2, A, B, E) :- E is min(2 * A, B).
