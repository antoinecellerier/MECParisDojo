namespace DiceScorerFSharp

type DiceScorer() = 
    let rec Rules (c: list<int * int>) =
        match c with
            | (1, c) :: xs when c >= 3 -> (1000<<<(c-3)) + Rules xs
            | (v, c) :: xs when c >= 3 -> v * (100<<<(c-3)) + Rules xs
            | (_,2) :: (_,2) :: (_,2) :: xs -> 800 + Rules xs
            | (_,1) :: (_,1) :: (_,1) :: (_,1) :: (_,1) :: (_,1) :: xs -> 1200 + Rules xs
            | (1,1) :: xs -> 100 + Rules xs
            | (5,1) :: xs -> 50 + Rules xs
            | x :: xs -> Rules xs
            | [] -> 0

    member this.Score (dice: seq<int>) =
        dice |> Seq.countBy id |> Seq.sortByDescending snd |> Seq.toList |> Rules
