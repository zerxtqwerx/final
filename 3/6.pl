solve :-
    read_int(T),
    solve_all(T).

solve_all(0).
solve_all(T) :-
    solve_one,
    T1 is T - 1,
    solve_all(T1).

solve_one :-
    read_int(N),
    read_list(N, A),
    read_list(N, B),
    find_ans(N, A, B, Ans),
    writeln(Ans).

read_int(X) :- read_line_to_string(user_input, S), atom_number(S, X).

read_list(N, L) :-
    read_line_to_string(user_input, S),
    split_string(S, " \t\n", "", Tokens),
    maplist(atom_number, Tokens, L),
    length(L, N).

find_ans(N, A, B, Ans) :-
    compute_positions(N, A, B, PosA, PosB),
    compute_next_prev(N, A, NextA, PrevA),
    compute_next_prev(N, B, NextB, PrevB),
    compute_max_reach(N, A, B, PosA, PosB, NextA, PrevA, NextB, PrevB, MaxReach),
    compute_sum(N, MaxReach, Ans).

compute_positions(N, A, B, PosA, PosB) :-
    length(PosA, N), length(PosB, N),
    fill_pos(N, A, PosA, 1),
    fill_pos(N, B, PosB, 1).

fill_pos(_, [], _, _).
fill_pos(N, [H|T], Pos, Idx) :-
    Idx <= N,
    replace(Pos, H, Idx),
    Idx1 is Idx + 1,
    fill_pos(N, T, Pos, Idx1).

replace(List, Index, Value) :-
    nth0(Index, List, _),
    replace_at(List, Index, Value, List).

replace_at([_|T], 0, X, [X|T]).
replace_at([H|T], I, X, [H|R]) :- I > 0, I1 is I - 1, replace_at(T, I1, X, R).

compute_next_prev(N, Arr, Next, Prev) :-
    length(Next, N), length(Prev, N),
    last_pos(N, Arr, N, Lp),
    fill_next(N, Arr, N, Lp, Next),
    first_pos(N, Arr, 1, Fp),
    fill_prev(N, Arr, 1, Fp, Prev).

last_pos(0, _, _, _).
last_pos(I, Arr, N, Lp) :-
    I > 0,
    nth0(I-1, Arr, Val),
    ( nth0(Val, Lp, L) -> true ; L = 0 ),
    replace(Lp, Val, I),
    I1 is I - 1,
    last_pos(I1, Arr, N, Lp).

fill_next(0, _, _, _, _).
fill_next(I, Arr, N, Lp, Next) :-
    I > 0,
    nth0(I-1, Arr, Val),
    nth0(Val, Lp, L),
    ( L = N + 1 -> NextVal = N + 1 ; NextVal = L ),
    replace(Next, I-1, NextVal),
    I1 is I - 1,
    fill_next(I1, Arr, N, Lp, Next).

first_pos(0, _, _, _).
first_pos(I, Arr, N, Fp) :-
    I <= N,
    nth0(I-1, Arr, Val),
    ( nth0(Val, Fp, F) -> true ; F = 0 ),
    replace(Fp, Val, I),
    I1 is I + 1,
    first_pos(I1, Arr, N, Fp).

fill_prev(N, Arr, I, Fp, Prev) :-
    I > N, !.
fill_prev(N, Arr, I, Fp, Prev) :-
    nth0(I-1, Arr, Val),
    nth0(Val, Fp, P),
    replace(Prev, I-1, P),
    I1 is I + 1,
    fill_prev(N, Arr, I1, Fp, Prev).

compute_max_reach(N, A, B, PosA, PosB, NextA, PrevA, NextB, PrevB, MaxReach) :-
    length(MaxReach, N),
    compute_reach(1, N, A, B, PosA, PosB, NextA, PrevA, NextB, PrevB, MaxReach, 1).

compute_reach(I, N, A, B, PosA, PosB, NextA, PrevA, NextB, PrevB, MaxReach, CurMin) :-
    I > N, !.
compute_reach(I, N, A, B, PosA, PosB, NextA, PrevA, NextB, PrevB, MaxReach, CurMin) :-
    nth0(I-1, A, Va),
    nth0(I-1, B, Vb),
    ( Va =:= Vb -> MR = I ; MR = 1 ),
    check_last(I, Va, Vb, PosA, PosB, MR1),
    check_prevA(I, PrevA, B, MR2),
    check_prevB(I, PrevB, A, MR3),
    check_crossA(I, Va, Vb, PosA, B, MR4),
    check_crossB(I, Va, Vb, PosB, A, MR5),
    MR0 is max(MR, MR1), MR0 is max(MR0, MR2), MR0 is max(MR0, MR3),
    MR0 is max(MR0, MR4), MR0 is max(MR0, MR5),
    replace(MaxReach, I-1, MR0),
    I1 is I + 1,
    compute_reach(I1, N, A, B, PosA, PosB, NextA, PrevA, NextB, PrevB, MaxReach, CurMin).

check_last(I, Va, Vb, PosA, PosB, Res) :-
    nth0(Va-1, PosA, PA),
    nth0(Vb-1, PosB, PB),
    ( PA < I, PB < I -> Res is max(PA, PB) + 1 ; Res = 1 ).

check_prevA(I, PrevA, B, Res) :-
    nth0(I-1, PrevA, Prev),
    ( Prev > 0, nth0(Prev-1, B, BPrev), nth0(I-1, B, BCur), BPrev =:= BCur -> Res is Prev + 1 ; Res = 1 ).

check_prevB(I, PrevB, A, Res) :-
    nth0(I-1, PrevB, Prev),
    ( Prev > 0, nth0(Prev-1, A, APrev), nth0(I-1, A, ACur), APrev =:= ACur -> Res is Prev + 1 ; Res = 1 ).

check_crossA(I, Va, Vb, PosA, B, Res) :-
    nth0(Vb-1, PosA, P),
    ( P > 0, P < I, nth0(P-1, B, BAtP), BAtP =:= Va -> Res is P + 1 ; Res = 1 ).

check_crossB(I, Va, Vb, PosB, A, Res) :-
    nth0(Va-1, PosB, P),
    ( P > 0, P < I, nth0(P-1, A, AAtP), AAtP =:= Vb -> Res is P + 1 ; Res = 1 ).

compute_sum(N, MaxReach, Ans) :-
    compute_sum_loop(1, N, MaxReach, 1, 0, Ans).

compute_sum_loop(R, N, MaxReach, CurMin, Acc, Ans) :-
    R > N, !, Ans = Acc.
compute_sum_loop(R, N, MaxReach, CurMin, Acc, Ans) :-
    nth0(R-1, MaxReach, MR),
    NewMin is max(CurMin, MR),
    Add is R - NewMin + 1,
    NewAcc is Acc + Add,
    R1 is R + 1,
    compute_sum_loop(R1, N, MaxReach, NewMin, NewAcc, Ans).

:- initialization(solve).