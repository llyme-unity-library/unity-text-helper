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
		OneMinus,
		/// <summary>
		/// [Value] - 1.0
		/// </summary>
		MinusOne,
		/// <summary>
		/// Shows the number sign when not zero.
		/// Only shows decimals if it has one.
		/// <br/>
		/// +1, -1, 0, +1.1, -0.1
		/// </summary>
		Stat,
		/// <summary>
		/// Shows the number sign when not zero.
		/// Only shows decimals if it has one.
		/// <br/>
		/// +1%, -1%, 0%, +1.1%, -0.1%
		/// </summary>
		StatPercent,
		Negate
	}
}
