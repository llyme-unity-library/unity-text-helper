namespace TextHelper
{
	public enum ExtendedFormat
	{
		Unknown,
		/// <summary>
		/// Uses `Mathf.Abs` if a number.
		/// </summary>
		Abs,
		/// <summary>
		/// 1.0 - [Value]
		/// </summary>
		PercentCompliment,
		/// <summary>
		/// [Value] - 1.0
		/// </summary>
		PercentMinusOne
	}
}
