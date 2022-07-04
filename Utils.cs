namespace Utils {
	public class Point {
		public int x;
		public int y;

		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override bool Equals(object? obj)
		{
			Point? p = obj as Point;

			if (p != null) {
				return this.x == p.x && this.y == p.y;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(x, y);
		}
	}
	
	public static class Extensions {
		public static T byPoint<T>(this T[,] arr, Point point) {
			return arr[point.x, point.y];
		}
		public static bool isInBounds<T>(this T[,] arr, Point point) {
			return
				point.x >= 0 && point.x < arr.GetLength(0) &&
				point.y >= 0 && point.y < arr.GetLength(0);
		}
		public static bool isInBounds<T>(this T[,] arr, int x, int y) {
			return
				x >= 0 && x < arr.GetLength(0) &&
				y >= 0 && y < arr.GetLength(0);
		}
		public static void For<T>(this T[,] arr, Action<int, int> action) {
			for (int x = 0; x < arr.GetLength(0); x++) {
				for (int y = 0; y < arr.GetLength(1); y++) {
					action(x, y);
				}
			}
		}
		public static void For<T>(this T[,] arr, Action<Point> action) {
			for (int x = 0; x < arr.GetLength(0); x++) {
				for (int y = 0; y < arr.GetLength(1); y++) {
					action(new Point(x, y));
				}
			}
		}

		public static IEnumerable<(T item, int index)> withIndex<T>(this IEnumerable<T> self) =>
			self.Select((item, index) => (item, index));

		public static bool isDigit(this int c) {
			return c >= 0 && c <= 9;
		} 

		public static T[] replaceAt<T>(this T[] arr, int index, T value) {
			var rtn = (T[]) arr.Clone();
			rtn[index] = value;
			return rtn;
		}

		public static List<T> join<T>(this List<T> first, List<T> second) {
			return first.Concat(second).ToList();
		}
	}
}