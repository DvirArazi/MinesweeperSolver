using Utils;

class Program {

	static int[,] puzzleStrToArr(string puzzleStr) {
		int width = puzzleStr.IndexOf('x');
		string trimmed = puzzleStr.Replace("x", "");

		var rtn = new int[width, trimmed.Length/width];

		for (int x = 0; x < rtn.GetLength(0); x++) {
			for (int y = 0; y < rtn.GetLength(1); y++) {
				var val = trimmed[x + width * y];
				rtn[x, y] = val >= '0' && val <= '9' ?
					val - '0' : val;
			}
		}

		return rtn;
	}

	static void printPuzzle(int[,] puzzle) {
		for (int y = 0; y < puzzle.GetLength(1); y++) {
			for (int x = 0; x < puzzle.GetLength(0); x++) {
				if (puzzle[x, y].isDigit()) {
					Console.Write(puzzle[x, y]);
				}
				else {
					Console.Write((char)puzzle[x, y]);
				}
			}
			Console.WriteLine();
		}
	}

	static void Main() {
		printPuzzle(Solver.solve(puzzleStrToArr(Puzzles.puzzle02), 0));
	}

}