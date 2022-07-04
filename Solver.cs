using Utils;

public static class Solver {

	class NumInfo {
		public int n;
		public List<int> relUnkIs = new List<int>();
	}

	static int optionInefficiency(int n, int m) {
		return n * (m-n);
	}

	public static int[,] solve(int[,] puzzle, int minesC) {
		int[,] solution = (int[,])puzzle.Clone();

		Point? prevCell = null;
		List<Point> relUnks = new List<Point>();
		List<Point> blacklist = new List<Point>();
		List<NumInfo> numInfos = new List<NumInfo>();

		List<Point> getRels(Point cell, Func<int, bool> cond) {
			var rtn = new List<Point>();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					var crnt = new Point(cell.x + x, cell.y + y);
					if (solution.isInBounds(crnt) && cond(solution.byPoint(crnt))) {
						rtn.Add(crnt);
					}
				}
			}

			return rtn;
		}
		List<Point> getRelUnks(Point cell) {
			return getRels(cell, (int rel) => rel == '#');
		}
		int getRelsC(Point cell, Func<int, bool> cond) {
			var rtn = 0;

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					var crnt = new Point(cell.x + x, cell.y + y);
					if (solution.isInBounds(crnt) && cond(solution.byPoint(crnt))) {
						rtn++;
					}
				}
			}

			return rtn;
		}
		int getRelUnksC(Point cell) {
			return getRelsC(cell, (int rel) => rel == '#');
		}
		int getRelFlgsC(Point cell) {
			return getRelsC(cell, (int rel) => rel == 'F');
		}
		Point? mostEfficientCellForPrev() {
			int getCommonUnkCs(Point first, Point second) {
				int getDistance(Point first, Point second) {
					return Math.Min(Math.Abs(first.x - second.x), Math.Abs(first.y - second.y));
				}

				int rtn = 0;

				foreach (var crnt in relUnks) {
					if (solution.isInBounds(crnt) && getDistance(crnt, second) == 1) {
						rtn++;
					}
				}

				return rtn;
			}

			Point? cell = null;
			int commonUnksC = 0;

			for (int x = -2; x <= 2; x++) {
				for (int y = -2; y <= 2; y++) {
					var crnt = new Point(prevCell.x + x, prevCell.y + y);
					int newCommonUnksC;
					if (solution.isInBounds(crnt) &&
						solution.byPoint(crnt).isDigit() && !blacklist.Contains(crnt) &&
						commonUnksC < (newCommonUnksC = getCommonUnkCs(prevCell, crnt))
					) {
						commonUnksC = newCommonUnksC;
						cell = crnt;
					}
				}
			}

			return cell;
		}
		Point? mostEfficientCell() {
			Point? cell = null;
			int ineff = Int32.MaxValue;

			solution.For((Point crnt) => {
				if (solution.byPoint(crnt).isDigit() && !blacklist.Contains(crnt)) {
					var crntIneff = optionInefficiency(solution.byPoint(crnt) - getRelFlgsC(crnt), getRelUnksC(crnt));
					if (ineff > crntIneff) {
						ineff = crntIneff;
						cell = crnt;
					}
				}
			});

			return cell;
		}
		List<int[]> getOptions() {
			List<int[]> inner(int[] option, int cursor) {
				//being used as a gate, only if the option is legal than the function will continue
				//otherwise return an empty list
				foreach (var numInfo in numInfos) {
					var flagsC = 0;
					var unksC = 0;
					foreach(var relUnkI in numInfo.relUnkIs) {
						switch(option[relUnkI]) {
							case 'F':
								flagsC++;
								break;
							case '#':
								unksC++;
								break;
						}
					}
					if (numInfo.n < flagsC || numInfo.n > flagsC + unksC) {
						return new List<int[]>();
					}
				}

				//check if the option is fully formed
				//if so, return it!
				//else, construct both options
				if (cursor == option.Length) {
					return new List<int[]>() { option };
				}

				var rtn = inner(option.replaceAt(cursor, '_'), cursor + 1);

				rtn = rtn.join(inner(option.replaceAt(cursor, 'F'), cursor + 1));

				return rtn;
			}

			return inner(Enumerable.Repeat((int)'#', relUnks.Count).ToArray(), 0);
		}

		while (true) {

			Point? num = null;

			//get the next cell to check, break if there are no more cells
			if (prevCell == null 
			|| (num = mostEfficientCellForPrev()) == null) {
				if ((num = mostEfficientCell()) == null) {
					break;
				}
			}

			//add all the unknowns surrounding the cell
			relUnks = relUnks.Union(getRelUnks(num)).ToList();

			//set the current numInfo and add it to the list
			var numInfo = new NumInfo();
				//Note: the next line would need to be passed to get options if it turns out I cannot
				//blacklist every cell after the first use.
				//might even split 'n' into two fields to avoid needlessly getting the number every time
			numInfo.n = solution.byPoint(num) - getRelFlgsC(num);
			var numRelUnks = getRelUnks(num);
			numInfo.relUnkIs = new List<int>(new int[numRelUnks.Count]);
			foreach(var (numRelUnk, i) in numRelUnks.withIndex()) {
				numInfo.relUnkIs[i] = relUnks.IndexOf(numRelUnk);
			}
			numInfos.Add(numInfo);

			//blacklist the chosen cell
				//Note: I'm still not sure if every cell is unsuable after iteration.
				// Change it later if I see that it misses checks
			blacklist.Add(num);

			//generate the options
			var options = getOptions();

			//check for common values in every option
			for (int i = 0; i < options[0].Length; i++) {
				for (int j = 1; j < options.Count; j++) {
					if (options[0][i] != options[j][i]) {
						options[0][i] = '#';
						break;
					}
				}
			}

			//add values to the solution
			for (int i = 0; i < relUnks.Count; i++) {
				solution[relUnks[i].x, relUnks[i].y] = options[0][i];
			}
			
			//check if any value changed
			bool changed = false;
			foreach (var val in options[0]) {
				if (val != '#') {
					changed = true;
					break;
				}
			}
			//if something changed, wipe the list of all the relevant unknowns + set prevCell to null
			//if not, (keep them to the next itteration of the loop) + set prevCell to current cell
			if (changed) {
				relUnks.Clear();
				numInfos.Clear();
				prevCell = null;
			}
			else {
				prevCell = num;
			}
		}

		return solution;
	}

}